namespace DeckSurf.SDK.Models;

/// <summary>
/// A marker interface for deck interaction events
/// </summary>
public interface IDeckEvent
{
}

/// <summary>
/// Event details for button press
/// </summary>
/// <param name="ButtonId">The id of the pressed button</param>
public record ButtonDown(int ButtonId) : IDeckEvent;

/// <summary>
/// Event details for button release
/// </summary>
/// <param name="ButtonId">The id of the released button</param>
public record ButtonUp(int ButtonId) : IDeckEvent;

/// <summary>
/// Event details for a screen tap
/// </summary>
/// <param name="X">The 'x' coordinate of the screen tap</param>
/// <param name="Y">The 'y' coordinate of the screen tap</param>
public record ScreenTap(int X, int Y) : IDeckEvent;

/// <summary>
/// Event details for a screen long-hold
/// </summary>
/// <param name="X">The 'x' coordinate of the screen long-hold</param>
/// <param name="Y">The 'y' coordinate of the screen long-hold</param>
public record ScreenHold(int X, int Y) : IDeckEvent;

/// <summary>
/// Event details for a screen swipe
/// </summary>
/// <param name="X1">The 'x' coordinate of the start of the swipe</param>
/// <param name="Y1">The 'y' coordinate of the start of the swipe</param>
/// <param name="X2">The 'x' coordinate of the end of the swipe</param>
/// <param name="Y2">The 'y' coordinate of the end of the swipe</param>
public record ScreenSwipe(int X1, int Y1, int X2, int Y2) : IDeckEvent;

/// <summary>
/// Event details for a knob press
/// </summary>
/// <param name="KnobId">The id of the pressed knob</param>
public record KnobDown(int KnobId) : IDeckEvent;

/// <summary>
/// Event details for a knob release
/// </summary>
/// <param name="KnobId">The id of the released knob</param>
public record KnobUp(int KnobId) : IDeckEvent;

/// <summary>
/// Event details for a knob clockwise rotation
/// </summary>
/// <param name="KnobId">The id of the rotated knob</param>
/// <param name="Clicks">The number of clicks rotated</param>
public record KnobClockwise(int KnobId, int Clicks) : IDeckEvent;

/// <summary>
/// Event details for a knob counterclockwise rotation
/// </summary>
/// <param name="KnobId">The id of the rotated knob</param>
/// <param name="Clicks">The number of clicks rotated</param>
public record KnobCounterClockwise(int KnobId, int Clicks) : IDeckEvent;