///
/// Using one file for all messages, beacuse there are not using a lot of messages and their data size is small
///

using Mirror;

public struct ReadyRequest : NetworkMessage
{
    public ReadyRequest(char color)
    {
        Color = color;
    }
    public char Color;
}

public struct ReadyResponse : NetworkMessage
{
    public ReadyResponse(bool accepted)
    {
        Accepted = accepted;
    }
    public bool Accepted;
}
