using System;

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
/// <param name="buttonId">The id of the pressed button</param>
public class ButtonDown(int buttonId) : EventArgs, IDeckEvent
{
    /// <summary>Gets the id of the pressed button</summary>
    public int ButtonId { get; } = buttonId;
}

/// <summary>
/// Event details for button release
/// </summary>
/// <param name="ButtonId">The id of the released button</param>
public class ButtonUp(int buttonId) : EventArgs, IDeckEvent
{
    /// <summary>
    /// Gets the id of the pressed button
    /// </summary>
    public int ButtonId { get; } = buttonId;
}

/// <summary>
/// Event details for a screen tap
/// </summary>
/// <param name="x">The 'x' coordinate of the screen tap</param>
/// <param name="y">The 'y' coordinate of the screen tap</param>
public class ScreenTap(int x, int y) : EventArgs, IDeckEvent
{
    /// <summary>Gets the 'x' coordinate of the screen tap</summary>
    public int X { get; } = x;

    /// <summary>Gets the 'y' coordinate of the screen tap</summary>
    public int Y { get; } = y;
}

/// <summary>
/// Event details for a screen long-hold
/// </summary>
/// <param name="x">The 'x' coordinate of the screen long-hold</param>
/// <param name="y">The 'y' coordinate of the screen long-hold</param>
public class ScreenHold(int x, int y) : EventArgs, IDeckEvent
{
    /// <summary>Gets the 'x' coordinate of the screen long-hold</summary>
    public int X { get; } = x;

    /// <summary>Gets the 'y' coordinate of the screen long-hold</summary>
    public int Y { get; } = y;
}

/// <summary>
/// Event details for a screen swipe
/// </summary>
/// <param name="x1">The 'x' coordinate of the start of the swipe</param>
/// <param name="y1">The 'y' coordinate of the start of the swipe</param>
/// <param name="x2">The 'x' coordinate of the end of the swipe</param>
/// <param name="y2">The 'y' coordinate of the end of the swipe</param>
public class ScreenSwipe(int x1, int y1, int x2, int y2) : EventArgs, IDeckEvent
{
    /// <summary>Gets the 'x' coordinate of the start of the swipe</summary>
    public int X1 { get; } = x1;

    /// <summary>Gets the 'y' coordinate of the start of the swipe</summary>
    public int Y1 { get; } = y1;

    /// <summary>Gets the 'x' coordinate of the end of the swipe</summary>
    public int X2 { get; } = x2;

    /// <summary>Gets the 'y' coordinate of the end of the swipe</summary>
    public int Y2 { get; } = y2;
}

/// <summary>
/// Event details for a knob press
/// </summary>
/// <param name="knobId">The id of the pressed knob</param>
public class KnobDown(int knobId) : EventArgs, IDeckEvent
{
    /// <summary>Gets the id of the pressed knob</summary>
    public int KnobId { get; } = knobId;
}

/// <summary>
/// Event details for a knob release
/// </summary>
/// <param name="knobId">The id of the released knob</param>
public class KnobUp(int knobId) : EventArgs, IDeckEvent
{
    /// <summary>Gets the id of the released knob</summary>
    public int KnobId { get; } = knobId;
}

/// <summary>
/// Event details for a knob clockwise rotation
/// </summary>
/// <param name="knobId">The id of the rotated knob</param>
/// <param name="clicks">The number of clicks rotated</param>
public class KnobClockwise(int knobId, int clicks) : EventArgs, IDeckEvent
{
    /// <summary>Gets the id of the rotated knob</summary>
    public int KnobId { get; } = knobId;

    /// <summary>Gets the number of clicks rotated</summary>
    public int Clicks { get; } = clicks;
}

/// <summary>
/// Event details for a knob counterclockwise rotation
/// </summary>
/// <param name="knobId">The id of the rotated knob</param>
/// <param name="clicks">The number of clicks rotated</param>
public class KnobCounterClockwise(int knobId, int clicks) : EventArgs, IDeckEvent
{
    /// <summary>Gets the id of the rotated knob</summary>
    public int KnobId { get; } = knobId;

    /// <summary>Gets the number of clicks rotated</summary>
    public int Clicks { get; } = clicks;
}