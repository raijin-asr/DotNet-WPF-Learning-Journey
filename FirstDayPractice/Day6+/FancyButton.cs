
using System.Windows;
using System.Windows.Controls;

namespace FirstDayPractice.Day6_
{
    public class FancyButton : Button
    {
        //register custom routed event
        public static readonly RoutedEvent GlowChangedEvent =
            EventManager.RegisterRoutedEvent(
                "GlowChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(FancyButton));

        public event RoutedEventHandler GlowChanged
        {
            add { AddHandler(GlowChangedEvent, value); }
            remove { RemoveHandler(GlowChangedEvent, value); }
        }

        protected override void OnClick()
        {
            base.OnClick();

            // Raise the custom event
            RoutedEventArgs args = new(GlowChangedEvent);
            RaiseEvent(args);
        }
    }
}
