using System.Windows.Input;

namespace Readme_Generator.Models;

public class CustomCommands
{
	private static readonly InputGestureCollection screenshotGesture = new() { new KeyGesture(Key.S, ModifierKeys.Control) };

	public static readonly RoutedUICommand Screenshot = new("Screenshot", "Screenshot", typeof(CustomCommands), screenshotGesture);
}
