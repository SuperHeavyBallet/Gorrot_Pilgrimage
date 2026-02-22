using UnityEngine;
using System;
using System.IO;

public class DeathPushNameToLedger : MonoBehaviour
{

    private const string LedgerFileName = "death_ledger.txt";

    public void FormPlayerInfo(string playerName, string playerHome, string playerDeathPlace)
    {
       PlayerInfo newPlayerInfo = new PlayerInfo();
        newPlayerInfo.playerName = playerName;
        newPlayerInfo.playerHome = playerHome;
        newPlayerInfo.playerDeathPlace = playerDeathPlace;

        string line = FormatLedgerLine(newPlayerInfo);

        Debug.Log($"{playerName} of {playerHome}, fell in {playerDeathPlace}");
        AppendToLedger(line);
    }

    private string FormatLedgerLine(PlayerInfo info)
    {
        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'");

        // Pipe-delimited makes it easy to parse later: split by '|'
        // Example:
        // 2026-02-22 01:23:45 UTC | Default Bob | Avbarnia | Semsun
        return $"{timestamp} | {info.playerName} | {info.playerHome} | {info.playerDeathPlace}{Environment.NewLine}";
    }

    private void AppendToLedger(string text)
    {
        string path = Path.Combine(Application.persistentDataPath, LedgerFileName);

        try
        {
            File.AppendAllText(path, text);
            Debug.Log($"Death ledger updated: {path}");
        }
        catch(Exception e )
        {
            Debug.LogError($"Failed to write death ledger at {path}\n{e}");
        }
    }

    public class PlayerInfo
    {
        public string playerName; 
        public string playerHome; 
        public string playerDeathPlace;
    }
}
