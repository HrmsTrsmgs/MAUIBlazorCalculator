using Marimo.WindowsCalculator.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Marimo.WindowsCalculator.WinUI3.Controls;


/// <summary>
/// ������\�����܂��B
/// </summary>
public sealed partial class HistoryListBox : UserControl
{
    /// <summary>
    /// <see cref="ItemsSource"/> �ˑ��֌W�v���p�e�B�̒�`�B
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<CalculationHistoryItem>), typeof(HistoryListBox),
        new PropertyMetadata(new ObservableCollection<CalculationHistoryItem>()));

    /// <summary>
    /// �{�^���̔w�i�F���擾�܂��͐ݒ肵�܂��B
    /// </summary>
    public IEnumerable<CalculationHistoryItem> ItemsSource
    {
        get => (IEnumerable<CalculationHistoryItem>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public HistoryListBox()
    {
        this.InitializeComponent();
    }
}