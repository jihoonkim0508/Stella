using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stage
{
    [Serializable]
    private class StageRow
    {
        [SerializeField] private int x = 1;
        [SerializeField] private int maxY = 1;

        public int X => Mathf.Max(1, x);
        public int MaxY => Mathf.Max(1, maxY);
    }

    [SerializeField] private int x = 1;
    [SerializeField] private int y = 1;
    [SerializeField] private List<StageRow> stageRows = new List<StageRow>();

    public int X => Mathf.Max(1, x);
    public int Y => Mathf.Clamp(y, 1, GetMaxY(X));

    public Stage()
    {
    }

    public Stage(int x, int y)
    {
        Set(x, y);
    }

    public void Set(int x, int y)
    {
        this.x = Mathf.Max(1, x);
        this.y = Mathf.Clamp(y, 1, GetMaxY(this.x));
    }

    public void Advance()
    {
        if (Y >= GetMaxY(X))
        {
            x = X + 1;
            y = 1;
            return;
        }

        y = Y + 1;
    }

    public string ToStageString()
    {
        return $"{X}-{Y}";
    }

    public override string ToString()
    {
        return ToStageString();
    }

    public static Stage operator ++(Stage stage)
    {
        if (stage == null)
        {
            return new Stage(1, 1);
        }

        stage.Advance();
        return stage;
    }

    private int GetMaxY(int stageX)
    {
        foreach (StageRow row in stageRows)
        {
            if (row != null && row.X == stageX)
            {
                return row.MaxY;
            }
        }

        return 1;
    }
}
