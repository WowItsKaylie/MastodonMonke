using ComputerInterface;
using ComputerInterface.ViewLib;

namespace MastodonMonke
{
    public class MyOtherModView : ComputerView
    {
        // This is called when you view is opened
        public override void OnShow(object[] args)
        {
            base.OnShow(args);
            // changing the Text property will fire an PropertyChanged event
            // which lets the computer know the text has changed and update it
            Text = "Beep Boop";
        }

        // you can do something on keypresses by overriding "OnKeyPressed"
        // it get's an EKeyboardKey passed as a parameter which wraps the old character string
        public override void OnKeyPressed(EKeyboardKey key)
        {
            switch (key)
            {
                case EKeyboardKey.Delete:
                    // "ReturnView" will basically "go back" and return to the last opened view
                    ReturnView();
                    break;
            }
        }
    }
}