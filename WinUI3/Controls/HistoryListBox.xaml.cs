using Marimo.WindowsCalculator.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
namespace Marimo.WindowsCalculator.WinUI3.Controls;


/// <summary>
/// ������\�����܂��B
/// </summary>
public sealed partial class HistoryListBox : UserControl, INotifyPropertyChanged
{
    /// <summary>
    /// <see cref="ItemsSource"/> �ˑ��֌W�v���p�e�B�̒�`�B
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<CalculationHistoryItem>), typeof(HistoryListBox),
        new PropertyMetadata(new ObservableCollection<CalculationHistoryItem>()));

    /// <summary>
    /// ���ڂ̈ꗗ���擾�܂��͐ݒ肵�܂��B
    /// </summary>
    public ReadOnlyObservableCollection<CalculationHistoryItem> ItemsSource
    {
        get => (ReadOnlyObservableCollection<CalculationHistoryItem>)GetValue(ItemsSourceProperty);
        set
        {
            SetValue(ItemsSourceProperty, value);
            if (ItemsSource is INotifyCollectionChanged source)
            {
                source.CollectionChanged += OnItemsSourceChanged;
            }
        }
    } 

    /// <summary>
    /// ItemsSource���󂩂ǂ������擾���܂��B
    /// </summary>
    public bool IsItemsSourceEmpty => !ItemsSource.Any();

    /// <summary>
    /// �v���p�e�B���ύX�����ƋN�����܂��B
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// ItemsSource���ύX�����ƌĂяo����܂��B
    /// </summary>
    /// <param name="sender">�C�x���g���N�����I�u�W�F�N�g�B</param>
    /// <param name="e">�C�x���g���B</param>
    private void OnItemsSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsItemsSourceEmpty)));
    }

    /// <summary>
    /// HistoryListBox�N���X�̐V�����C���X�^���X�����������܂��B
    /// </summary>
    public HistoryListBox()
    {
        InitializeComponent();
    }
}