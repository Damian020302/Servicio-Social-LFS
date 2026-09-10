using System.Collections.Generic;
using UnityEngine;

public static class GameMaster
{
    public static string archivePath = "C:\\LANR\\";
    private static string patientName;
    public static string PatientName
    {
        get { return patientName; }
    }

    private static string patientID;
    public static string PatientID
    {
        get { return patientID; }
    }

    public static void SetPatientName(string name)
    {
        patientName = name;
    }

    public static void SetPatientID(string id)
    {
        patientID = id;
    }

    public static void SetPatientNameAndId(string _name, string _id)
    {
        patientName = _name;
        patientID = _id;
    }

    public static void ResetPatientData()
    {
        patientName = "";
        patientID = "";
    }

    public static int sessionsPlayed;
    public static int SessionsPlayed
    {
        get { return sessionsPlayed; }
    }

    public static void RegisterSessionsPlayed(int sessions)
    {
        sessionsPlayed = sessions;
    }

    public static string password;

    private static string therapistName;
    public static string TherapistName
    {
        get { return therapistName; }
    }

    private static string therapistID;
    public static string TherapistID
    {
        get { return therapistID; }
    }

    public static void SetTherapistNameAndId(string _name, string _id)
    {
        therapistName = _name;
        therapistID = _id;
    }

    public static void ResetTherapistData()
    {
        therapistName = "";
        therapistID = "";
    }

    ////////////////////////////////////////////////////////

    private static int game;
    public static Hand selectedHand = Hand.Right;
    public static Hand SelectedHand
    {
        get { return selectedHand; }
    }

    ////////////////////////////////////////////////////////
    
    public enum Hand
    {
        Left,
        Right,
        Both
    }

    public class SessionData
    {
        public string name;
        public int id;
        public List<Game1> Game1OnThisSession = new List<Game1>();
        //public List<Game2> Game2OnThisSession = new List<Game2>();
        //public List<Game3> Game3OnThisSession = new List<Game3>();
        //public List<Game4> Game4OnThisSession = new List<Game4>();
    }

    public class PlayerData
    {
        public string name;
        public int id;
        public List<SessionData> sessions = new List<SessionData>();
    }

    public class TherapistData
    {
        public string Name;
        public int Id;
    }

    public class Game1
    {
        public float totalRoundTime;
        public string date;
        public string roundStartingTime;
        public float maxSpeedAchieved;
        public float avgSpawningTimeAchieved;
        public int score;
        public int missed;
        public float initialReactionTimeL;
        public float initialReactionTimeR;
        public float avgArmAngleL;
        public float avgArmAngleR;
        public int leftInteractions;
        public int rightInteractions;
        //Checar si se usa Hand o string para la mano
    }

    ////////////////////////////////////////////////////////


}
