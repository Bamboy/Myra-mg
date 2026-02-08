using System.Collections.Generic;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Properties;

namespace Myra.Samples.Inspector
{
    public class RootWidgets : HorizontalStackPanel
    {
        public Label labelOverGui;
        private PropertyGrid propertyGrid;

        public readonly List<object> inspectables;
        
        public RootWidgets()
        {
            inspectables = BuildInspectables();
            BuildUI();
            Inspect(inspectables[0]);
        }

        private List<object> BuildInspectables()
        {
            return new List<object>()
            {
                new SomeTypesInAClass(),
                this,
                InspectGame.Instance
            };
        }

        private void BuildUI()
        {
            labelOverGui = new Label();
            labelOverGui.Width = 400;
            labelOverGui.Padding = new Thickness(4);
            this.Widgets.Add(labelOverGui);
            
            propertyGrid = new PropertyGrid();
            propertyGrid.Width = 500;
            this.Widgets.Add(propertyGrid);
        }
        
        public void Inspect(object obj)
        {
            propertyGrid.Object = obj;
        }
    }
}