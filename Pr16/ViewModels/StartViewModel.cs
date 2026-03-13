using System.ComponentModel;
using System.Runtime.CompilerServices;

public class StartViewModel : INotifyPropertyChanged
{
    private string _playerName = "Герой";
    public string PlayerName
    {
        get => _playerName;
        set { _playerName = value; OnPropertyChanged(); }
    }

    private bool _selectedSword = true;
    public bool SelectedSword
    {
        get => _selectedSword;
        set { _selectedSword = value; OnPropertyChanged(); }
    }

    public bool SelectedClaymore { get; set; }
    public bool SelectedAxe { get; set; }

    private bool _selectedHeavyArmor = true;
    public bool SelectedHeavyArmor
    {
        get => _selectedHeavyArmor;
        set { _selectedHeavyArmor = value; OnPropertyChanged(); }
    }

    public bool SelectedMediumArmor { get; set; }
    public bool SelectedLightArmor { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}