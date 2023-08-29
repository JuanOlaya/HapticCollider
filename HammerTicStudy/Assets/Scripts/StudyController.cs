using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public enum Bedingung
{
    HammerController_Nail_HN,  // Nail_HammerTic
    HammerController_Glass_HG,  // Nail_ViveController
    ViveController_Nail_VN,  // Glass_HammerTic
    ViveController_Glass_VG,   // Glass_ViveController
    Training
};
/*
public enum AufgabeCopy
{
    Brush,
    Drehen,
    Oel
};
*/

/// <summary>
/// object holds study conditions
/// </summary>
/// 
//LaterStudySetupHammer or something like this!!!!!!!!!!!!!!!!!!!
[System.Serializable]
public class StudySetup
{
    [SerializeField]
    [Tooltip("Die Bedingung auf die getestet wird: TakeOver vs Guidance")]
    public Bedingung conditionType = Bedingung.Training;
    public int condition=0;
    //[Tooltip("Typ Aufgaben")]
    //[SerializeField]
    //public Aufgabe TaskType;
    [Tooltip("Wahr wenn Aufgabe fertiggestellt")]
    public bool TaskDone = false;
    [Tooltip("Zeit die fuer die Fertigstellung der Aufgabe gebraucht wurde")]
    public long T1start;
    public long T2start;
    public long T1end;
    public long T2end;
    public float oelValue;
    public float pressureValue;
    [Tooltip("laufende Nummer")]
    public int index = 0;

    public Vector3 hammerImpactLocation1;
    public Vector3 hammerImpactLocation2;
    public Vector3 hammerImpactLocation3;
    public Vector3 hammerImpactLocation4;
    public Vector3 hammerImpactLocation5;

    public Vector3 targetPosition1;
    public Vector3 targetPosition2;
    public Vector3 targetPosition3;
    public Vector3 targetPosition4;
    public Vector3 targetPosition5;

    public float hammerImpactSpeed1;
    public float hammerImpactSpeed2;
    public float hammerImpactSpeed3;
    public float hammerImpactSpeed4;
    public float hammerImpactSpeed5;

    public float distanceAfterImpact1;
    public float distanceAfterImpact2;
    public float distanceAfterImpact3;
    public float distanceAfterImpact4;
    public float distanceAfterImpact5;
}


/// <summary>
/// run and register studies
/// </summary>
public class StudyController : MonoBehaviour
{
    public GameObject glassy;
    public GameObject naily;
    public GameObject blocky;

    public GameObject VR_camera;

    public GameObject glass;
    public GameObject nail;
    public GameObject block;

    [Tooltip("laufende Nummer: Proband")]
    public int UserID;

    [Tooltip("Initialisiere alle relevanten Informationen fuer einen Studiendurchlauf")]
    public List<StudySetup> studies;          // Why are you storing the studies in a List???
    public CSV Writer;                       // What was this used for???

    private CSV table;

    public Vector3 hammerImpactLocation1;
    public Vector3 hammerImpactLocation2;
    public Vector3 hammerImpactLocation3;
    public Vector3 hammerImpactLocation4;
    public Vector3 hammerImpactLocation5;

    public Vector3 targetPosition1;
    public Vector3 targetPosition2;
    public Vector3 targetPosition3;
    public Vector3 targetPosition4;
    public Vector3 targetPosition5;

    public int hammerImpactCounter = 0;

    public float hammerImpactSpeed1;
    public float hammerImpactSpeed2;
    public float hammerImpactSpeed3;
    public float hammerImpactSpeed4;
    public float hammerImpactSpeed5;

    public float distanceAfterImpact1;
    public float distanceAfterImpact2;
    public float distanceAfterImpact3;
    public float distanceAfterImpact4;
    public float distanceAfterImpact5;

    private List<StudySetup> tempStudies;       //Change listener to update VR controller
    private Coroutine aufgabeCoroutine;
    int num = 0;
    //private bool takeOverZeichnen = false;
    private bool freeZeichnen = false;
    private bool takeOverDampf = false;
    private bool takeOverOel = false;
    //private bool guideZeichnen = false;
    //private bool guideDampf = false;
    private bool guideOel = false;
    private bool freeDampf = false;
    private bool freeOel = false;

    private bool aloneZeichnen = false;
    private bool aloneDampf = false;
    private bool aloneOel = false;
    //public bool zwei = false;
    public bool guide = false;

    public long t1Start;
    public long t2Start;
    public long t1End;
    public long t2End;

    public float startPressureValue = 1.0f;
    //private SteamEngineController sc;
    private GameObject steamEngine;
    //private DSVR_Tropfoeler to;
    private GameObject tropfoeler;
    private GameObject tempLine;


    public GameObject InitLineObj;
    public LineRenderer currLine;
    private GameObject line;
    public List<string> coordinates = new List<string>();
    private List<Vector3> vectors = new List<Vector3>();

    public StudySetup studySetup;

    private float tempPressure;

    public GameObject NailGlassBlock;

    //private String fileLocation = "C:\\Users\\GEVAKUB\\Desktop\\MinfTogetherStudie\\outputData\\Data\\";
    //private String fileLocation = "C:\\Users\\juan\\Documents\\UnityProjects\\HammerTicStudy\\HammerTicStudy\\Logs\\DataStudy\\";

    private String fileLocation = "C:\\Users\\Tilt Brush\\Documents\\HCI-projects\\HammerTicStudy-20220404T191127Z-001\\HammerTicStudy\\HammerTicStudy\\Logs\\DataStudy\\";

    /*
    public DSVR_Tropfoeler To
    {
        get
        {
            return to;
        }

        set
        {
            to = value;
        }
    }
    */

    private void Awake()
    {
        /*
        steamEngine = GameObject.Find("SteamEngine");
        sc = steamEngine.GetComponent<SteamEngineController>();
        tropfoeler = GameObject.Find("OelLow_Study");
        to = tropfoeler.GetComponent<DSVR_Tropfoeler>();
        */
        studySetup = new StudySetup();
    }


    private void Start()
    {
        //glass = GameObject.Find("Glass");
        //nail = GameObject.Find("Naily");

        naily.SetActive(false);
        glassy.SetActive(false);
        blocky.SetActive(true);
    }

    public static long getCurrentTimeMillis()
    {
        DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return (long)(DateTime.UtcNow - startTime).TotalMilliseconds;
    }

    private void Update()
    {
        //Studien starten
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            studySetup.condition = 1;
            RunStudy(studySetup);
            t1Start = getCurrentTimeMillis();
            
            naily.SetActive(true);
            glassy.SetActive(false);
            blocky.SetActive(false);
            //takeOverZeichnen = true;
            //Painter = new CSV();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            studySetup.condition = 2;
            RunStudy(studySetup);
            t1Start = getCurrentTimeMillis();
            
            naily.SetActive(false);
            glassy.SetActive(true);
            blocky.SetActive(false);

            //takeOverDampf = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            studySetup.condition = 3;
            RunStudy(studySetup);
            t1Start = getCurrentTimeMillis();
            
            naily.SetActive(true);
            glassy.SetActive(false);
            blocky.SetActive(false);

            //takeOverOel = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            studySetup.condition = 4;
            RunStudy(studySetup);
            t1Start = getCurrentTimeMillis();
            
            naily.SetActive(false);
            glassy.SetActive(true);
            blocky.SetActive(false);

            //guideZeichnen = true;
            //guide = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            studySetup.condition = 0;
            //RunStudy(studySetup);
            //t1Start = getCurrentTimeMillis();

            nail.SetActive(false);
            glassy.SetActive(false);
            blocky.SetActive(true);
            studySetup.conditionType = Bedingung.Training;

            //guideZeichnen = true;
            //guide = true;
        }

        // Studie beenden
        if (Input.GetKeyDown(KeyCode.E))
        {
            endCondition();
            //guide = false;

        }
        //Debug.Log("studySetup.condition: "+ studySetup.condition);
        //Debug.Log("hammerBlowCounter: " + hammerBlowCounter);

        if (Input.GetKeyDown(KeyCode.G)) {
            glassy.GetComponent<Rigidbody>().useGravity = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            naily.transform.position = new Vector3(-1.0466f, 1.276f, 0.2758f);
            hammerImpactCounter = 0;
            //Destroy
            GameObject[] glassHole = GameObject.FindGameObjectsWithTag("GlassHole");
            //Debug.Log("GlassHole.Length: " + glassHole.Length);
            foreach (GameObject go in glassHole)
            {
                Destroy(go);
            }
            NailGlassBlock.SetActive(true);
            naily.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            //Debug.Log("Entraqq");
            nail.transform.position = new Vector3(nail.transform.position.x, VR_camera.transform.position.y, nail.transform.position.z);
            glass.transform.position = new Vector3(glass.transform.position.x, VR_camera.transform.position.y, glass.transform.position.z);
        }

        studySetup.hammerImpactLocation1 = hammerImpactLocation1;
        studySetup.hammerImpactLocation2 = hammerImpactLocation2;
        studySetup.hammerImpactLocation3 = hammerImpactLocation3;
        studySetup.hammerImpactLocation4 = hammerImpactLocation4;
        studySetup.hammerImpactLocation5 = hammerImpactLocation5;

        studySetup.targetPosition1 = targetPosition1;
        studySetup.targetPosition2 = targetPosition2;
        studySetup.targetPosition3 = targetPosition3;
        studySetup.targetPosition4 = targetPosition4;
        studySetup.targetPosition5 = targetPosition5;

        studySetup.hammerImpactSpeed1 = hammerImpactSpeed1;
        studySetup.hammerImpactSpeed2 = hammerImpactSpeed2;
        studySetup.hammerImpactSpeed3 = hammerImpactSpeed3;
        studySetup.hammerImpactSpeed4 = hammerImpactSpeed4;
        studySetup.hammerImpactSpeed5 = hammerImpactSpeed5;

        studySetup.distanceAfterImpact1 = distanceAfterImpact1;
        studySetup.distanceAfterImpact2 = distanceAfterImpact2;
        studySetup.distanceAfterImpact3 = distanceAfterImpact3;
        studySetup.distanceAfterImpact4 = distanceAfterImpact4;
        studySetup.distanceAfterImpact5 = distanceAfterImpact5;

        //Debug.Log("studySetup.condition: " + studySetup.condition);
    }

    public void endCondition()
    {
        studySetup.TaskDone = true;
        t1End = getCurrentTimeMillis();
        //t2End = getCurrentTimeMillis();
        
        num++;
        
        WriteWhenDone(studySetup);

    }

    /// <summary>
    /// start study with given index
    /// </summary>
    /// <param name="i"> study index </param>
    public void StarteStudieWithIndex()         // Are you invoking this method from another class/script? 
    {
        RunStudy(studySetup);
    }

    /// <summary>
    /// start study conditions in gameController
    /// </summary>
    /// <param name="study"></param>
    /// 

    private void RunStudy(StudySetup study)    
    {
        /*
        if (takeOverZeichnen)
        {
            study.conditionType = Bedingung.Nail_HammerTic;
            //study.TaskType = Aufgabe.Brush;
        }
        if (takeOverDampf)
        {
            study.conditionType = Bedingung.Nail_ViveController;
            //study.TaskType = Aufgabe.Drehen;
        }
        */
        if (study.condition == 1)
        {
            study.conditionType = Bedingung.HammerController_Nail_HN;
            //study.TaskType = Aufgabe.Brush;
        }
        if (study.condition == 2)
        {
            study.conditionType = Bedingung.HammerController_Glass_HG;
            //study.TaskType = Aufgabe.Drehen;
        }
        if (study.condition == 3)
        {
            study.conditionType = Bedingung.ViveController_Nail_VN;
            //study.TaskType = Aufgabe.Drehen;
        }
        if (study.condition == 4)
        {
            study.conditionType = Bedingung.ViveController_Glass_VG;
            //study.TaskType = Aufgabe.Drehen;
        }
        /*
        if (study.condition == 5)
        {
            study.conditionType = Bedingung.Training;
            //study.TaskType = Aufgabe.Drehen;
        }
        */
    }

    /// <summary>
    /// end study in gameController
    /// </summary>
    /// <param name="study"></param>
    public void EndStudy(StudySetup study)
    {
        //takeOverZeichnen = false;
        freeZeichnen = false;
        takeOverDampf = false;
        takeOverOel = false;
        //guideZeichnen = false;
        //guideDampf = false;
        guideOel = false;
        freeDampf = false;
        freeOel = false;

        aloneZeichnen = false;
        aloneDampf = false;
        aloneOel = false;

        Debug.Log("Aufgabe beendet");
    }
    /// <summary>
    /// start study and wait for master to mark it done in VR or inspector
    /// </summary>
    /// <param name="study"></param>
    /// <returns></returns>
    private void WriteWhenDone(StudySetup study)
    {
        study.T1start = t1Start;
        study.T1end = t1End;
        study.T2start = t2Start;
        study.T2end = t2End;
        study.pressureValue = -1.0f;
        study.oelValue = -1;
        if (takeOverOel)
        {
            //study.oelValue = to.fillValue;
        }
        if (guideOel)
        {
            //study.oelValue = to.fillValue;
        }
        if (freeOel)
        {
            //study.oelValue = to.fillValue;
        }
        if (aloneOel)
        {
            //study.oelValue = to.fillValue;
        }

        EndStudy(study);

        if (table == null)
            table = new CSV(UserID, study);
        table.SaveCSV(study);
    }

    /// <summary>
    /// Add a new indexed study 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public StudySetup AddStudy(int index)     // Are you invoking this method from another class/script? 
    {
        //studies.Add(new StudySetup());
        //StudySetup study = studies[index];
        //study.index = index;
        return (studySetup);
    }
    /*
    public void storePressure()
    {
        tempPressure = sc.PressureValue;
    }
    */
}
