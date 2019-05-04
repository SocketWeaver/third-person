using System;
using System.Collections.Generic;

[Serializable]
public class PlayerScores
{
    public List<Score> scores = new List<Score>();
}

[Serializable]
public class Score
{
    public string playerRemoteId;
    public int score;
}