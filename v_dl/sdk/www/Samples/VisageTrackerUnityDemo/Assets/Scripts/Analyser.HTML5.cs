using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public partial class Analyser
{
#if UNITY_WEBGL
    #region Properties

    /// <summary>
    /// Results of the performed face analysis will be stored in a 12-element float array i.e. AnalysisData in the following order: age, ageValid, gender, genderValid, emotionProbabilities and emotionsValid. 
    /// The AnalysisDataIndices is a helper class for iterating over the AnalysisData array.
    /// </summary>
    static class AnalysisDataIndices {
        public const int Age = 0;
        public const int AgeValid = 1;
        public const int Gender = 2;
        public const int GenderValid = 3;
        public const int EmotionProbabilities = 4;
        public const int EmotionsValid = 11;
    }

    bool AnalyserInitialized = false;

    [Header("Face Analysis output data")]
    const int NUM_EMOTIONS = 7;
    private const double EMO_DISPLAY_TRESHOLD = 0.3;
    private float[] age = new float[Tracker.MAX_FACES];
    private int[] gender = new int[Tracker.MAX_FACES];
    private List<float[]> EmotionListPerFace = new List<float[]>();

    [Header("Tracker object settings")]
    private int[] TrackerStatus = new int[Tracker.MAX_FACES];
    private Vector3[] RotationApparent = new Vector3[Tracker.MAX_FACES];
    private float[] rotationApparent = new float[3];

    [Header("Analysis Data Element")]
    List<GameObject> AnalysisList = new List<GameObject>();

    //Gender display variables
    private Material[] matrialList;
    private Material[] warningList;

    //Emotion display variables
    private Vector3[][] emoVertices = new Vector3[Tracker.MAX_FACES][];
    private float EmoBarSize;
    private TextMesh[][] percentage = new TextMesh[Tracker.MAX_FACES][];

    FDP[] fdpArray = new FDP[Tracker.MAX_FACES];
    private float[] rawfdp = new float[2000];

    #endregion

    // Use this for initialization
    void Start()
    {
        string dataFilesPath = Application.streamingAssetsPath + "/" + "Visage Tracker/vfa";

        switch (Application.platform)
        {
            case RuntimePlatform.IPhonePlayer:

                break;
            case RuntimePlatform.Android:
                dataFilesPath = Application.persistentDataPath + "/" + "vfa";

                break;
            case RuntimePlatform.OSXPlayer:

                break;
            case RuntimePlatform.OSXEditor:

                break;
            case RuntimePlatform.WebGLPlayer:

                break;
            case RuntimePlatform.WindowsEditor:
                dataFilesPath = Application.streamingAssetsPath + "/Visage Tracker/vfa";
                break;
        }

        // Initialize analysis 
        AnalyserInitialized = InitializeAnalyser();

        // Initialize arrays used for face analysis
        InitializeContainers();

        // Initialize apparent rotation array
        for (int i = 0; i < Tracker.MAX_FACES; i++)
        {
            RotationApparent[i] = new Vector3(0, 0, -1000);
            EmotionListPerFace.Add(default(float[]));
        }

        warningPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (AnalyserInitialized && Tracker.frameForAnalysis)
        {
            UpdateAnalysisResults();
        }
    }

    /// <summary>
    /// Initialize analyser
    /// </summary>
    bool InitializeAnalyser()
    {
        VisageTrackerNative._initAnalyser("initAnalyserCallback");
        return AnalyserInitialized;
    }

    /// <summary>
    /// Callback function is called when FA is initialised.
    /// </summary>
    void initAnalyserCallback()
    {
        Debug.Log("AnalyserInited");
        AnalyserInitialized = true;
    }

    /// <summary>
	/// Initialize arrays used for face analysis.
	/// </summary>
    private void InitializeContainers()
    {
        for (int i = 0; i < Tracker.MAX_FACES; ++i)
        {
            age[i] = -1.0f;
            gender[i] = -1;
            EmotionListPerFace.Add(default(float[]));
            AnalysisList.Add((GameObject)Instantiate(AnalysisDataElement));
            AnalysisList[i].GetComponent<MeshFilter>().transform.position -= new Vector3(0, 0, 10000);

            fdpArray[i] = new FDP();

            emoVertices[i] = AnalysisList[i].GetComponentsInChildren<MeshFilter>()[3 + i].mesh.vertices;

            percentage[i] = new TextMesh[NUM_EMOTIONS];

            for (int j = 0; j < NUM_EMOTIONS; ++j)
            {
                percentage[i][j] = AnalysisList[i].GetComponentsInChildren<TextMesh>()[1 + j];
            }
        }

        matrialList = Resources.LoadAll("Analysis/Material", typeof(Material)).Cast<Material>().ToArray();

        warningList = Resources.LoadAll("WarningPanel/Material", typeof(Material)).Cast<Material>().ToArray();

        EmoBarSize = Math.Abs(emoVertices[0][0].x - emoVertices[0][3].x);
    }

    private void UpdateAnalysisResults()
    {
        VisageTrackerNative._getTrackerStatus(TrackerStatus);
        for (int i = 0; i < Tracker.MAX_FACES; i++)
        {
            if (TrackerStatus[i] != (int)TrackStatus.OK)
            {
                ResetAnalysis(i);
                RemoveAnalysisDataElement(i);
            }

            if (TrackerStatus[i] == (int)TrackStatus.OK)
            {
                PerformAnalysis(i);

                DrawEmotions(i, EmotionListPerFace[i]);

                DrawGender(i, gender[i]);

                DrawAge(i, age[i]);

                DisplayAnalysisResults(i);
            }
        }
    }

    /// <summary>
    /// Removes element with analysis data.
    /// </summary>
    /// <param name="faceIndex">Index for the particular tracked face.</param>
    private void RemoveAnalysisDataElement(int faceIndex)
    {
        AnalysisList[faceIndex].GetComponent<MeshFilter>().transform.position -= new Vector3(0, 0, 10000);
    }

    /// <summary>
    /// Performs face analysis for a given face (faceIndex).
    /// </summary>
    /// <param name="faceIndex">Index of the tracker/face for which face analysis will be performed.</param>
    private void PerformAnalysis(int faceIndex)
    {
        float[] AnalysisData = new float[12];
        
        int analyserOptions = (int)(VFAFlags.VFA_AGE | VFAFlags.VFA_GENDER | VFAFlags.VFA_EMOTION);

        VisageTrackerNative._analyseStream(faceIndex, analyserOptions, AnalysisData);

        // check if age, gender and emotion estimations are valid
        if (!Convert.ToBoolean(AnalysisData[AnalysisDataIndices.AgeValid]) || !Convert.ToBoolean(AnalysisData[AnalysisDataIndices.GenderValid]) || !Convert.ToBoolean(AnalysisData[AnalysisDataIndices.EmotionsValid]))
            return;

       age[faceIndex] = AnalysisData[AnalysisDataIndices.Age];
       gender[faceIndex] = (int)AnalysisData[AnalysisDataIndices.Gender];
       EmotionListPerFace[faceIndex] = new float[NUM_EMOTIONS];

       for (int i = 0; i < NUM_EMOTIONS; i++)
            EmotionListPerFace[faceIndex][i] = AnalysisData[i+AnalysisDataIndices.EmotionProbabilities];
    }

    /// <summary>
    /// Resets analysis parameters.
    /// Typical usage is when tracker status has changed from TrackStatus.OK to TrackStatus.INIT
    /// </summary>
    /// <param name="faceIndex">Index for the particular tracked face.</param>
    private void ResetAnalysis(int faceIndex)
    {
        age[faceIndex] = -1;
        gender[faceIndex] = -1;
        EmotionListPerFace[faceIndex] = default(float[]);
        VisageTrackerNative._resetStreamAnalysis(faceIndex);
    }

    /// <summary>
    /// Adjust emotion bars according to emotion analysis values for a given tracker/face (faceIndex).
    /// If the passed Emotions array is null, no bars will be drawn.
    /// </summary>
    /// <param name="faceIndex">Index of the tracker/face for which to draw emotions</param>
    /// <param name="Emotions">Float array containing emotions</param>
    private void DrawEmotions(int faceIndex, float[] Emotions)
    {
        for (int emoIndex = 0; emoIndex < NUM_EMOTIONS; emoIndex++)
        {
            emoVertices[faceIndex][0].x = emoVertices[faceIndex][2].x;
            emoVertices[faceIndex][1].x = emoVertices[faceIndex][2].x;

            if (Emotions == null)
                return;

            float EmoBarLength = EmoBarSize * Emotions[emoIndex];
            if (Emotions[emoIndex] > EMO_DISPLAY_TRESHOLD)
            {
                emoVertices[faceIndex][0].x = emoVertices[faceIndex][2].x + EmoBarLength;
                emoVertices[faceIndex][1].x = emoVertices[faceIndex][2].x + EmoBarLength;
            }
            AnalysisList[faceIndex].GetComponentsInChildren<MeshFilter>()[3 + emoIndex].mesh.vertices = emoVertices[faceIndex];

            if (Emotions[emoIndex] > EMO_DISPLAY_TRESHOLD)
            {
                percentage[faceIndex][emoIndex].text = ((int)(Emotions[emoIndex] * 100)).ToString() + "%";
            }
            else
            {
                percentage[faceIndex][emoIndex].text = " ";
            }
        }
    }

    /// <summary>
    /// Adjust material on gender mesh according to gender analysis values for a given tracker/face (faceIndex).
    /// </summary>
    /// <param name="faceIndex">Index of the tracker/face for which to draw gender</param>
    /// <param name="gender">0 for female, 1 for male</param>
    private void DrawGender(int faceIndex, int gender)
    {
        AnalysisList[faceIndex].GetComponentsInChildren<Renderer>()[3].enabled = true;
        if (gender == 0)
        {
            AnalysisList[faceIndex].GetComponentsInChildren<Renderer>()[3].GetComponentsInChildren<Renderer>()[0].material = matrialList[0];
            AnalysisList[faceIndex].GetComponentsInChildren<Renderer>()[3].GetComponentsInChildren<Renderer>()[0].enabled = true;

        }
        else if (gender == 1)
        {
            AnalysisList[faceIndex].GetComponentsInChildren<Renderer>()[3].GetComponentsInChildren<Renderer>()[0].material = matrialList[1];
            AnalysisList[faceIndex].GetComponentsInChildren<Renderer>()[3].GetComponentsInChildren<Renderer>()[0].enabled = true;

        }
        else
        {
            AnalysisList[faceIndex].GetComponentsInChildren<Renderer>()[3].enabled = false;
        }
    }


    /// <summary>
    /// Adjust text on age text mesh to age value for a given tracker/face (faceIndex).
    /// </summary>
    /// <param name="faceIndex">Index of the tracker/face for which to draw age</param>
    /// <param name="age">estimated age for given faceIndex</param>
    private void DrawAge(int faceIndex, float age)
    {
        if (age > 0)
        {
            AnalysisList[faceIndex].GetComponentsInChildren<Renderer>()[1].GetComponent<TextMesh>().text = (int)Math.Round(age, 0) + "y";
        }
        else
        {
            AnalysisList[faceIndex].GetComponentsInChildren<Renderer>()[1].GetComponent<TextMesh>().text = " - ";
        }

    }

    /// <summary>
    /// Calculates the chin point position and sets analysis data element at that position for a given tracker/face (faceIndex).
    /// </summary>
    /// <param name="faceIndex">Index of the tracker/face for which analysis data will be displayed</param>
    private void DisplayAnalysisResults(int faceIndex)
    {
        float[] positions = new float[3];

        VisageTrackerNative._getAllFeaturePoints3D(rawfdp, rawfdp.Length, faceIndex);

        fdpArray[faceIndex].Fill(rawfdp);

        Vector3[] positions3D = new Vector3[Tracker.MAX_FACES];

        // Get position of chin point - 2.1
        positions = fdpArray[faceIndex].getFPPos(2, 1);

        positions3D[faceIndex].x = -positions[0];
        positions3D[faceIndex].y = positions[1];
        positions3D[faceIndex].z = positions[2];

        AnalysisList[faceIndex].GetComponent<MeshFilter>().transform.position = positions3D[faceIndex];

        if (!withinConstraints(faceIndex))
        {

            warningPanel.GetComponentInChildren<Image>().material = warningList[1];
            warningPanel.SetActive(true);
            warningPanelImage.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Lerp(warningPanelImage.color.a, 235 / 255f, Time.deltaTime * 3f));
        }
        else
        {
            warningPanelImage.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Lerp(warningPanelImage.color.a, 0.0f, Time.deltaTime * 3f));
        }
    }

    /// <summary>
    /// Compares obtained apparent rotation data with set constraints if data is within the limits returns true, otherwise returns false
    /// </summary>
    /// <param name="faceIndex">Index of the tracker/face for which analysis data will be displayed</param>
    public bool withinConstraints(int faceIndex)
    {
        VisageTrackerNative._getHeadRotationApparent(rotationApparent, faceIndex);

        RotationApparent[faceIndex].x = rotationApparent[0];
        RotationApparent[faceIndex].y = rotationApparent[1];
        RotationApparent[faceIndex].z = rotationApparent[2];

        double HeadPitchCompensatedRad = RotationApparent[faceIndex].x;
        double HeadYawCompensatedRad = RotationApparent[faceIndex].y;
        double HeadRollRad = RotationApparent[faceIndex].z;
        
        double HeadPitchCompensatedDeg = HeadPitchCompensatedRad * Mathf.Rad2Deg;
        double HeadYawCompensatedDeg = HeadYawCompensatedRad * Mathf.Rad2Deg;
        double HeadRollDeg = HeadRollRad * Mathf.Rad2Deg;

        const double CONSTRAINT_ANGLE = 40;
        
        if (Math.Abs(HeadPitchCompensatedDeg) > CONSTRAINT_ANGLE ||
            Math.Abs(HeadYawCompensatedDeg) > CONSTRAINT_ANGLE ||
            Math.Abs(HeadRollDeg) > CONSTRAINT_ANGLE ||
            VisageTrackerNative._getFaceScale(faceIndex) < 40)
            return false;

        return true;
    }
#endif
}
