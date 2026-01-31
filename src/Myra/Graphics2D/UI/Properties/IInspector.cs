namespace Myra.Graphics2D.UI.Properties
{
    public interface IInspector
    {
        //object? SelectionRoot { get; set; }
        object SelectedField { get; }
        void FireChanged(string propertyName);
    }
}