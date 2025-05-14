using System.Configuration;
using System.Data;
using System.Windows;
using static ChessWPF.MainWindow;

namespace ChessWPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public BoardTheme CurrentTheme { get; set; } = BoardTheme.Gray;
}

