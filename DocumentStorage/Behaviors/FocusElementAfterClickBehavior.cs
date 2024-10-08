using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace DocumentStorage.Behaviors
{
    public class FocusElementAfterClickBehavior : Behavior<ButtonBase>
    {
        private ButtonBase _associatedButton;

        public Control FocusElement
        {
            get => (Control)GetValue(FocusElementProperty);
            set => SetValue(FocusElementProperty, value);
        }

        public static readonly DependencyProperty FocusElementProperty =
            DependencyProperty.Register(
                "FocusElement", typeof(Control),
                typeof(FocusElementAfterClickBehavior),
                new UIPropertyMetadata());

        protected override void OnAttached()
        {
            base.OnAttached();
            _associatedButton = AssociatedObject;
            _associatedButton.Click += AssociatedButtonClick;
        }
        protected override void OnDetaching()
        {
            _associatedButton.Click -= AssociatedButtonClick;
            base.OnDetaching();
        }

        private void AssociatedButtonClick(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(FocusElement);
        }
    }
}
