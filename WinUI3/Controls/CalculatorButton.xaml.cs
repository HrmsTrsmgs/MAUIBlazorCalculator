using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Windows.Input;

namespace Marimo.WindowsCalculator.WinUI3.Controls;

/// <summary>
/// �v�Z�@�Ŏg�p�����{�^����\���܂��B
/// </summary>
/// <remarks>
/// ���̃R���g���[���́A�R�}���h�A�e�L�X�g�A�w�i�F���w�肷�邱�Ƃ��ł��܂��B
/// </remarks>
public sealed partial class CalculatorButton : UserControl
{
    /// <summary>
    /// <see cref="Command"/> �ˑ��֌W�v���p�e�B�̒�`�B
    /// </summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register("Command", typeof(ICommand), typeof(CalculatorButton),
        new PropertyMetadata(null));

    /// <summary>
    /// �{�^�������s����R�}���h���擾�܂��͐ݒ肵�܂��B
    /// </summary>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// <see cref="Text"/> �ˑ��֌W�v���p�e�B�̒�`�B
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(CalculatorButton),
        new PropertyMetadata(""));

    /// <summary>
    /// �{�^���ɕ\������e�L�X�g���擾�܂��͐ݒ肵�܂��B
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// <see cref="BackGround"/> �ˑ��֌W�v���p�e�B�̒�`�B
    /// </summary>
    public static readonly DependencyProperty BackGroundProperty =
        DependencyProperty.Register("BackGround", typeof(Brush), typeof(CalculatorButton),
        new PropertyMetadata(new SolidColorBrush(Colors.White)));

    /// <summary>
    /// �{�^���̔w�i�F���擾�܂��͐ݒ肵�܂��B
    /// </summary>
    public Brush BackGround
    {
        get => (Brush)GetValue(BackGroundProperty);
        set => SetValue(BackGroundProperty, value);
    }

    /// <summary>
    /// <see cref="CalculatorButton"/> CalculatorButton�N���X�̐V�����C���X�^���X�����������܂��B
    /// </summary>
    public CalculatorButton()
    {
        this.InitializeComponent();
    }
}
