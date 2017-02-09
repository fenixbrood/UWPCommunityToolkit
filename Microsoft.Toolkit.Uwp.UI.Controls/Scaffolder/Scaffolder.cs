namespace Microsoft.Toolkit.Uwp.UI.Controls.Scaffolder
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Class Scaffolder. 
    /// </summary>
    /// <typeparam name="T">The model to scaffold. Should implement interface INotifyProperyChanged</typeparam>
    public class Scaffolder<T>
    {
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public T Model { get; private set; }

        /// <summary>
        /// The type of Model 
        /// </summary>
        private Type modelType = typeof(T);

        /// <summary>
        /// Initializes a new instance of the <see cref="Scaffolder" /> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public Scaffolder(T model)
        {
            Model = model;
        }

        /// <summary>
        /// Applies the model.
        /// </summary>
        /// <param name="panel">The panel.</param>
        /// <param name="filter">The filter.</param>
        public void ApplyModel(Panel panel, string filter = "")
        {
            var controls = GetFrameworkElementsForModel(filter);
            foreach (var c in controls)
            {
                panel.Children.Add(c);
            }
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>System.String.</returns>
        public string GetHeader(PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<DisplayAttribute>(true);
            return attr != null ? attr.Name : property.Name;
        }

        /// <summary>
        /// Gets the order of the models property
        /// </summary>
        /// <param name="property">The property with DisplayAttribute</param>
        /// <seealso cref="DisplayAttribute"/>
        /// <returns>System.Int32.</returns>
        public int GetOrder(PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<DisplayAttribute>(true);
            return attr != null ? attr.Order : 0;
        }

        /// <summary>
        /// Gets the autogenerate.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>System.Boolean.</returns>
        public bool GetAutogenerate(PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<DisplayAttribute>(true);
            return attr != null ? attr.AutoGenerateField : true;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>System.String.</returns>
        public string GetDescription(PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<DisplayAttribute>(true);
            return attr == null ? attr.Description : null;
        }

        /// <summary>
        /// Gets the watermark.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>System.String.</returns>
        public string GetWatermark(PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<DisplayAttribute>(true);
            return attr == null ? attr.Prompt : string.Empty;
        }

        /// <summary>
        /// Gets the framework elements for model.
        /// </summary>
        /// <param name="filter">The filter based on DisplayAttribute.Name.</param>
        /// <seealso cref="DisplayAttribute"/>
        /// <returns>System.Collections.Generic.IEnumerable&lt;Windows.UI.Xaml.FrameworkElement&gt;.</returns>
        public IEnumerable<FrameworkElement> GetFrameworkElementsForModel(string filter = "")
        {
            var props = modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.Name.Contains(filter))
                .OrderBy(p => GetOrder(p));
            foreach (var control in props.SelectMany(x => GetFrameworkElements(x)))
            {
                yield return control;
            }
        }

        /// <summary>
        /// Gets the framework elements.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>IEnumerable&lt;FrameworkElement&gt;.</returns>
        public IEnumerable<FrameworkElement> GetFrameworkElements(PropertyInfo property)
        {
            if (!GetAutogenerate(property))
            {
                yield break;
            }

            var values = Enum.GetNames(property.PropertyType);

            if (property.PropertyType.IsAssignableFrom(typeof(bool)))
            {
                yield return GetControlBool(property);
            }
            else if (property.PropertyType.IsAssignableFrom(typeof(string)))
            {
                yield return GetControlString(property);
            }
            else if (property.PropertyType.IsAssignableFrom(typeof(DateTimeOffset)))
            {
                yield return GetControlDateTimeOffset(property);
            }
            else if (property.PropertyType.IsAssignableFrom(typeof(int)))
            {
                yield return GetControlInt(property);
            }

            var description = GetDescription(property);
            if (description != null)
            {
                yield return new TextBlock() { Text = description };
            }
        }

        /// <summary>
        /// Gets the control int.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>Windows.UI.Xaml.FrameworkElement.</returns>
        private FrameworkElement GetControlInt(PropertyInfo property)
        {
            return GetControlString(property);
        }

        /// <summary>
        /// Gets the control date time offset.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>Windows.UI.Xaml.FrameworkElement.</returns>
        private FrameworkElement GetControlDateTimeOffset(PropertyInfo property)
        {
            var control = new DatePicker();
            control.Header = GetHeader(property);
            var binding = new Binding()
            {
                Source = Model,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath(property.Name)
            };
            control.SetBinding(DatePicker.DateProperty, binding);
            return control;
        }

        /// <summary>
        /// Gets the control string.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>Windows.UI.Xaml.FrameworkElement.</returns>
        private FrameworkElement GetControlString(PropertyInfo property)
        {
            var control = new TextBox();
            control.Header = property.Name;

            var binding = new Binding()
            {
                Source = Model,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath(property.Name)
            };
            control.SetBinding(TextBox.TextProperty, binding);
            return control;
        }

        /// <summary>
        /// Gets the control bool.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>Windows.UI.Xaml.FrameworkElement.</returns>
        private FrameworkElement GetControlBool(PropertyInfo property)
        {
            var control = new ToggleSwitch();
            control.Header = property.Name;
            var binding = new Binding()
            {
                Source = Model,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath(property.Name)
            };
            control.SetBinding(ToggleSwitch.IsOnProperty, binding);
            return control;
        }
    }
}
