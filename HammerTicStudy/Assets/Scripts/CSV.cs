using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class CSV : MonoBehaviour
{

    private StudySetup studie;
    private int UserId;
    private int studyId;
    private string studieBedingung;
    private long t1s;
    private long t1e;
    private long t2s;
    private long t2e;
    private float fillValue;

    private float hammerImpactLocation1X;
    private float hammerImpactLocation1Y;
    private float hammerImpactLocation1Z;

    private float hammerImpactLocation2X;
    private float hammerImpactLocation2Y;
    private float hammerImpactLocation2Z;

    private float hammerImpactLocation3X;
    private float hammerImpactLocation3Y;
    private float hammerImpactLocation3Z;

    private float hammerImpactLocation4X;
    private float hammerImpactLocation4Y;
    private float hammerImpactLocation4Z;

    private float hammerImpactLocation5X;
    private float hammerImpactLocation5Y;
    private float hammerImpactLocation5Z;

    private float targetPosition1X;
    private float targetPosition1Y;
    private float targetPosition1Z;

    private float targetPosition2X;
    private float targetPosition2Y;
    private float targetPosition2Z;

    private float targetPosition3X;
    private float targetPosition3Y;
    private float targetPosition3Z;

    private float targetPosition4X;
    private float targetPosition4Y;
    private float targetPosition4Z;

    private float targetPosition5X;
    private float targetPosition5Y;
    private float targetPosition5Z;

    private float hammerImpactSpeed1;
    private float hammerImpactSpeed2;
    private float hammerImpactSpeed3;
    private float hammerImpactSpeed4;
    private float hammerImpactSpeed5;

    private float distanceAfterImpact1;
    private float distanceAfterImpact2;
    private float distanceAfterImpact3;
    private float distanceAfterImpact4;
    private float distanceAfterImpact5;

    //private String fileLocation = "C:\\Users\\GEVAKUB\\Desktop\\MinfTogetherStudie\\outputData\\";

    //private String fileLocation = "C:\\Users\\juan\\Documents\\UnityProjects\\HammerTicStudy\\HammerTicStudy\\Logs\\DataStudy\\";
    //private String fileLocation = "C:\\Users\\Tilt Brush\\Documents\\HCI-projects\\HammerTicStudy-20220404T191127Z-001\\HammerTicStudy\\HammerTicStudy\\Logs\\DataStudy\\";

    private String fileLocation = "C:\\Users\\Tilt Brush\\Documents\\Unity Projects\\HammerTicStudy-20220404T191127Z-001\\HammerTicStudy\\HammerTicStudy\\Logs\\DataStudy\\";

    //private String fileLocation = " D:\\UnityData\\";

    private StreamWriter studieDataWriter;

    //create CSV object pass all relevant information
    public CSV(int UserId, StudySetup studie)
    {
        this.UserId = UserId;
        this.studie = studie;
        this.studieBedingung = studie.conditionType.ToString();
        t1s = studie.T1start;
        t1e = studie.T1end;
        t2s = studie.T2start;
        t2e = studie.T2end;
        fillValue = studie.oelValue;
        //...

        hammerImpactLocation1X = studie.hammerImpactLocation1.x;
        hammerImpactLocation1Y = studie.hammerImpactLocation1.y;
        hammerImpactLocation1Z = studie.hammerImpactLocation1.z;

        hammerImpactLocation2X = studie.hammerImpactLocation2.x;
        hammerImpactLocation2Y = studie.hammerImpactLocation2.y;
        hammerImpactLocation2Z = studie.hammerImpactLocation2.z;

        hammerImpactLocation3X = studie.hammerImpactLocation3.x;
        hammerImpactLocation3Y = studie.hammerImpactLocation3.y;
        hammerImpactLocation3Z = studie.hammerImpactLocation3.z;

        hammerImpactLocation4X = studie.hammerImpactLocation4.x;
        hammerImpactLocation4Y = studie.hammerImpactLocation4.y;
        hammerImpactLocation4Z = studie.hammerImpactLocation4.z;

        hammerImpactLocation5X = studie.hammerImpactLocation5.x;
        hammerImpactLocation5Y = studie.hammerImpactLocation5.y;
        hammerImpactLocation5Z = studie.hammerImpactLocation5.z;

        targetPosition1X = studie.targetPosition1.x;
        targetPosition1Y = studie.targetPosition1.y;
        targetPosition1Z = studie.targetPosition1.z;

        targetPosition2X = studie.targetPosition2.x;
        targetPosition2Y = studie.targetPosition2.y;
        targetPosition2Z = studie.targetPosition2.z;

        targetPosition3X = studie.targetPosition3.x;
        targetPosition3Y = studie.targetPosition3.y;
        targetPosition3Z = studie.targetPosition3.z;

        targetPosition4X = studie.targetPosition4.x;
        targetPosition4Y = studie.targetPosition4.y;
        targetPosition4Z = studie.targetPosition4.z;

        targetPosition5X = studie.targetPosition5.x;
        targetPosition5Y = studie.targetPosition5.y;
        targetPosition5Z = studie.targetPosition5.z;

        hammerImpactSpeed1 = studie.hammerImpactSpeed1;
        hammerImpactSpeed2 = studie.hammerImpactSpeed2;
        hammerImpactSpeed3 = studie.hammerImpactSpeed3;
        hammerImpactSpeed4 = studie.hammerImpactSpeed4;
        hammerImpactSpeed5 = studie.hammerImpactSpeed5;

        distanceAfterImpact1 = studie.distanceAfterImpact1;
        distanceAfterImpact2 = studie.distanceAfterImpact2;
        distanceAfterImpact3 = studie.distanceAfterImpact3;
        distanceAfterImpact4 = studie.distanceAfterImpact4;
        distanceAfterImpact5 = studie.distanceAfterImpact5;

        createWriters();
    }
    public CSV()
    {
        
    }
    private void dataHolder(StudySetup studie)
    {
        this.studie = studie;
        this.studieBedingung = studie.conditionType.ToString();
        t1s = studie.T1start;
        t1e = studie.T1end;
        t2s = studie.T2start;
        t2e = studie.T2end;
        fillValue = studie.oelValue;
    }


    private long getCurrentTimeMillis()
    {
        DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return (long)(DateTime.UtcNow - startTime).TotalMilliseconds;
    }

    private void createWriters()
    {
        //looks for existing files and increases the version counter
        int dataWriterCounter = 0;

        while (true)
        {
            if (File.Exists(fileLocation + "Data/" + "U" + UserId + "Data_" + dataWriterCounter + ".csv"))
            {
                Debug.Log("The file does exist! dataWriter++");
                dataWriterCounter++;
            }
            else
            {
                break;
            }
        }


        //creating save writers
        Debug.Log("Creating data writer...");
        studieDataWriter = new System.IO.StreamWriter(fileLocation + "Data/" + "U" + UserId + "Data_" + dataWriterCounter + ".csv", true);
        studieDataWriter.WriteLine("take start: " + getCurrentTimeMillis());
        studieDataWriter.WriteLine("UserId;Timestamp;Condition;T1Start;T1End;T2Start;T2End;HammerImpactLocation1.x;HammerImpactLocation1.y;HammerImpactLocation1.z;HammerImpactLocation2.x;HammerImpactLocation2.y;HammerImpactLocation2.z;HammerImpactLocation3.x;HammerImpactLocation3.y;HammerImpactLocation3.z;HammerImpactLocation4.x;HammerImpactLocation4.y;HammerImpactLocation4.z;HammerImpactLocation5.x;HammerImpactLocation5.y;HammerImpactLocation5.z;targetPosition1.x;targetPosition1.y;targetPosition1.z;targetPosition2.x;targetPosition2.y;targetPosition2.z;targetPosition3.x;targetPosition3.y;targetPosition3.z;targetPosition4.x;targetPosition4.y;targetPosition4.z;targetPosition5.x;targetPosition5.y;targetPosition5.z;HammerImpactSpeed1;HammerImpactSpeed2;HammerImpactSpeed3;HammerImpactSpeed4;HammerImpactSpeed5;DistanceAfterImpact1;DistanceAfterImpact2;DistanceAfterImpact3;DistanceAfterImpact4;DistanceAfterImpact5");
        studieDataWriter.Flush();
        Debug.Log("Successfully created data writer.");
    }

    public void SaveCSV(StudySetup study)
    {
        dataHolder(study);
        //String result = UserId.ToString() + ";" + getCurrentTimeMillis().ToString() + ";" + studieBedingung + ";" + t1s + ";" + t1e + ";" + t2s + ";" + t2e + ";" + fillValue.ToString();
        String result = UserId.ToString() + ";" + getCurrentTimeMillis().ToString() + ";" + studieBedingung + ";" + t1s + ";" + t1e + ";" + t2s + ";" + t2e +";" + hammerImpactLocation1X + ";" + hammerImpactLocation1Y + ";" + hammerImpactLocation1Z + ";" + hammerImpactLocation2X + ";" + hammerImpactLocation2Y + ";" + hammerImpactLocation2Z + ";" + hammerImpactLocation3X + ";" + hammerImpactLocation3Y + ";" + hammerImpactLocation3Z + ";" + hammerImpactLocation4X + ";" + hammerImpactLocation4Y + ";" + hammerImpactLocation4Z + ";" + hammerImpactLocation5X + ";" + hammerImpactLocation5Y + ";" + hammerImpactLocation5Z + ";" + targetPosition1X + ";" + targetPosition1Y + ";" + targetPosition1Z + ";" + targetPosition2X + ";" + targetPosition2Y + ";" + targetPosition2Z + ";" + targetPosition3X + ";" + targetPosition3Y + ";" + targetPosition3Z + ";" + targetPosition4X + ";" + targetPosition4Y + ";" + targetPosition4Z + ";" + targetPosition5X + ";" + targetPosition5Y + ";" + targetPosition5Z + ";" + hammerImpactSpeed1 + ";" + hammerImpactSpeed2 + ";" + hammerImpactSpeed3 + ";" + hammerImpactSpeed4 + ";" + hammerImpactSpeed5 + ";" + distanceAfterImpact1 + ";" + distanceAfterImpact2 + ";" + distanceAfterImpact3 + ";" + distanceAfterImpact4 + ";" + distanceAfterImpact5;
        writeData(result);
    }

    public void writeData(String result)
    {

        studieDataWriter.WriteLine(result);
        studieDataWriter.Flush();

    }
}
