﻿using Marimo.WindowsCalculator.Models.Calculations;
using System.Collections.ObjectModel;

namespace Marimo.WindowsCalculator.Models;

/// <summary>
/// 電卓を表します。
/// </summary>
public class Calculator : ModelBase
{
    /// <summary>
    /// 現在行っている計算です。
    /// </summary>
    Calculation activeCaluculation = new NumberCalculation(null);

    /// <summary>
    /// 履歴作成のための計算です。
    /// </summary>
    ObservableCollection<Calculation> cumulativeCalculation = new();

    /// <summary>
    /// 現在、最新で行われている計算を取得、設定します。
    /// </summary>
    /// <remarks>
    /// 現在、最新で行われている計算を取得、設定します。
    /// 設定された値のレシーバーは、直前の計算に設定されます。
    /// </remarks>
    public Calculation ActiveCaluculation
    {
        get => activeCaluculation;
        private set
        {
            if (activeCaluculation == value) return;
            value.Receiver = activeCaluculation;
            SetProperty(ref activeCaluculation, value);
        }
    }

    /// <summary>
    /// Redo用に取ってある計算を取得、設定します。
    /// </summary>
    public Calculation? RedoCalculation { get; set; } = null;

    /// <summary>
    /// 履歴用の計算結果一覧を取得します。
    /// </summary>
    public IEnumerable<CalculationHistoryItem> CalculationHistory
    {
        get
        {
            foreach(var calculation in cumulativeCalculation.Reverse())
            {
                if (calculation.Result != null && calculation is OperationCalculation)
                {
                    var operation = calculation as OperationCalculation;
                    yield return new(
                        $"{operation!.CurrentExpression} {operation.Operand} =",
                        operation.Result!.Value);
                }
            }
        }
    }

    /// <summary>
    /// 計算結果を取得します。
    /// </summary>
    public string DisplaiedNumber
    {
        get
        {
            try
            {
                return
                    (ActiveCaluculation switch
                    {
                        NumberCalculation c => c.NumberToken.Number,
                        OperationCalculation c
                            => (c.Result ?? c.Operand?.Number ?? c.Receiver?.Result
                                ) ??
                                throw new InvalidOperationException(
                                    "今の演算も前の演算も結果が出てないのはおかしいはず"),
                        EqualButtonCalculation c
                            => c.Result,
                        DeleteCalculation c
                            => c.Result,
                        _ => 0

                    } ?? throw new InvalidOperationException()).ToString("N");
            }
            catch (DivideByZeroException)
            {
                return "0 で割ることはできません";
            }
        }
    }
    /// <summary>
    /// Calculatorクラスの新しいインスタンスを初期化します。
    /// </summary>
    public Calculator()
    {
        PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName != nameof(ActiveCaluculation)) return;

            if(!cumulativeCalculation.Contains(ActiveCaluculation))
            {
                cumulativeCalculation.Add(ActiveCaluculation);
            }
            else
            {
                while(cumulativeCalculation.Last() != ActiveCaluculation)
                {
                    cumulativeCalculation.Remove(cumulativeCalculation.Last());
                }
            }
        };
    }

    /// <summary>
    /// 履歴を削除します。
    /// </summary>
    public void ClearCalculationHistory()
    {
        cumulativeCalculation.Clear();
    }

    /// <summary>
    /// 電卓にトークンを入力します。
    /// </summary>
    /// <param name="token">入力されたトークン。</param>
    public void Input(Token token)
    {
        switch (token)
        {
            case NumberToken t:
                InputNumberToken(t);
                break;
            case OperatorToken t:
                switch(ActiveCaluculation)
                {
                    case OperationCalculation c when c.Operand == null:
                        c.OperatorToken = t.Operator;
                        break;
                    case OperationCalculation c:
                        c.IsDisplaiedResult = true;
                        goto default;
                    default:
                        ActiveCaluculation
                            = new OperationCalculation(ActiveCaluculation, t.Operator, null);
                        break;
                }
                
                break;
            case OtherToken t:
                switch(t.Kind)
                {
                    case OtherTokenKind.Undo:
                        if (RedoCalculation == null)
                        {
                            RedoCalculation = ActiveCaluculation;
                        }
                        if (ActiveCaluculation.Receiver == null) break;
                        SetProperty(ref activeCaluculation!, ActiveCaluculation.Receiver, nameof(ActiveCaluculation));
                        switch(ActiveCaluculation)
                        {
                            case OperationCalculation c:
                                c.IsDisplaiedResult = true; 
                                break;
                        }
                        break;
                    case OtherTokenKind.Redo:
                        if (RedoCalculation != null)
                        {
                            var nextCalculation = RedoCalculation;
                            while (nextCalculation!.Receiver != ActiveCaluculation)
                            {
                                nextCalculation = nextCalculation!.Receiver;
                            }
                            SetProperty(ref activeCaluculation, nextCalculation, nameof(ActiveCaluculation));
                        }
                        break;
                    case OtherTokenKind.Equal:
                        switch(ActiveCaluculation)
                        {
                            case OperationCalculation c:
                                c.IsDisplaiedResult = true;
                                break;
                        }
                        goto default;
                    case OtherTokenKind.C:
                        ActiveCaluculation = new NumberCalculation(null);
                        ClearCalculationHistory();
                        break;
                    case OtherTokenKind.CE:
                        switch(ActiveCaluculation)
                        {
                            case NumberCalculation c:
                                c.NumberToken = new(0);
                                break;
                            default:
                                ActiveCaluculation = new NumberCalculation(ActiveCaluculation);
                                break;
                        }
                        break;
                    default: 
                        ActiveCaluculation = GetCalculationOtherWhenTokenInputed(t);
                        break;
                };
                break;
        }
        OnPropertyChanged(nameof(DisplaiedNumber));
    }

    /// <summary>
    /// 数値と演算子以外の入力を行います。
    /// </summary>
    private Calculation GetCalculationOtherWhenTokenInputed(OtherToken otherToken)
        => otherToken.Kind switch
        {
            OtherTokenKind.Equal =>
                new EqualButtonCalculation(
                    ActiveCaluculation,
                    ActiveCaluculation switch
                    {
                        EqualButtonCalculation c => GetLastOperationCalculation(c),
                        _ => null
                    }),
            OtherTokenKind.C =>
                new DeleteCalculation(ActiveCaluculation),
            _ => throw new NotSupportedException()
        };

    /// <summary>
    /// 次の最後の演算を取得します。
    /// </summary>
    /// <param name="lastCaluculation">これまでの最後の計算</param>
    /// <returns>これまでの最後の演算。</returns>
    private OperationCalculation? GetLastOperationCalculation(
        EqualButtonCalculation lastCaluculation)
    {
        var before
            = lastCaluculation.LastOperationCalculation
            ?? lastCaluculation.Receiver as OperationCalculation
            ?? throw new InvalidOperationException();
        return new OperationCalculation(
                ActiveCaluculation, before.OperatorToken, before.Operand);
    }

    /// <summary>
    /// 数値トークンが入力された処理を行います。
    /// </summary>
    /// <param name="token">数値トークン。</param>
    private void InputNumberToken(NumberToken token)
    {
        switch (ActiveCaluculation)
        {
            case NumberCalculation c:
                c.NumberToken = token;
                break;
            case OperationCalculation c:
                c.Operand = token;
                break;
        }
    }

}
