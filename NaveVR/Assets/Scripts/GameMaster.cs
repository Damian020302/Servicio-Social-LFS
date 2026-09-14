using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GameMaster
{
    public static string archivePath = "C:\\LANR\\";
    public static bool INDAUTOR = false;
    public static void CreateDirectory()
    {
        if(!Directory.Exists(archivePath)) Directory.CreateDirectory(archivePath);
    }

    private static string _fileName = "";
    public static string FileName
    {
        get { return _fileName; }
    }
    public static void SetFileName(string patientId)
    {
        _fileName = patientId + "_Data.xml";
    }

    private static string patientSessionRecordPath = "";
    public static string PatientSessionRecordPath
    {
        get { return patientSessionRecordPath; }
    }
    public static void SetPatientSessionRecordPath(string patientId)
    {
        SetFileName(patientId);
        patientSessionRecordPath = archivePath + "\\" + _fileName;
    }

    ////////////////////////////////////////////////////////
    private static string offlineTherapist = "clinica";
    public static string OfflineTherapist
    {
        get { return offlineTherapist; }
    }
    private static string offlineTherapistPassword = "clinica";
    public static string OfflineTherapistPassword
    {
        get { return offlineTherapistPassword; }
    }
    ////////////////////////////////////////////////////////
    
    private static string patientName;
    public static string PatientName
    {
        get { return patientName; }
    }

    private static string patientId;
    public static string PatientId
    {
        get { return patientId; }
    }

    public static void SetPatientName(string name)
    {
        patientName = name;
    }

    public static void SetPatientID(string id)
    {
        patientId = id;
    }

    public static void SetPatientNameAndId(string _name, string _id)
    {
        patientName = _name;
        patientId = _id;
    }

    public static void ResetPatientData()
    {
        patientName = "";
        patientId = "";
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
        public List<Game2> Game2OnThisSession = new List<Game2>();
        public List<Game3> Game3OnThisSession = new List<Game3>();
        public List<Game4> Game4OnThisSession = new List<Game4>();
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
        public string currentArmName;
        public float totalRoundTime;
        public string date;
        public string roundStartingTime;
        public float maxSpeedAchieved;
        public float avgSpawningTimeAchieved;
        public int score;
        public int missed;
        public float initialReactionTimeL;
        public float avgInteractionTimeL;
        public float maxRadiusAchievedL;
        public float avgArmAngleL;
        public int enemiesTouchedL;
        public float initialReactionTimeR;
        public float avgInteractionTimeR;
        public float maxRadiusAchievedR;
        public float avgArmAngleR;
        public int enemiesTouchedR;
    }

    public class Game2
    {
        public float totalRoundTime;
        public string date;
        public string roundStartingTime;
        public string currentArmName;
        public float initialReactionTime;
        public float avgRotationTime;
        public float avgSolvingTime;
        public float avgRotationAngle;
        public float maxRotationAngle;
    }

    public class Game3
    {
        public float totalRoundTime;
        public string date;
        public string roundStartingTime;
        public string currentArmName;
        public float initialReactionTime;
        public float avgFlexionTime;
        public float maxFlexionAngle;
        public float avgFlexionAngle;
        public float avgExtensionTime;
        public float maxExtensionAngle;
        public float avgExtensionAngle;
    }

    public class Game4
    {
        public float totalRoundTime;
        public string date;
        public string roundStartingTime;
        public string currentArmName;
        public float initialReactionTime;
        public float avgHoldTime;
        public float avgGrabTime;
        public float totalGrabbedRobots;
        public int collectedRobots;
        public int droppedRobots;
    }
    ////////////////////////////////////////////////////////


}
