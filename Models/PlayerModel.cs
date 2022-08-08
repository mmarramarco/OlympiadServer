/// <summary>
/// The player model.
/// </summary>
public class PlayerModel
{
    /// <summary>
    /// The pseudo of the player.
    /// </summary>
    /// <value></value>
    public string Pseudo { get; set; }

    /// <summary>
    /// The peer id of the player.
    /// </summary>
    /// <value></value>
    public int PeerId { get; set; }

    /// <summary>
    /// Is the player ready to start the match?
    /// </summary>
    /// <value></value>
    public bool IsReady { get; set; }

    /// <summary>
    /// Is it the first player?
    /// </summary>
    /// <value></value>
    public bool IsFirstPlayer { get; set; }
}
