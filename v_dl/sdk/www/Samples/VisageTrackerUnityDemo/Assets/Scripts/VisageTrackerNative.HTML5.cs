using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;


public static partial class VisageTrackerNative
{
#if UNITY_WEBGL

    #region Tracker

    /** This function initialises the tracker.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _initTracker(string config, int MAX_FACES, string callbackInitTracker);
    
    /** This function sets the path from which *.data and visageSDK.wasm files will be loaded.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _setDataPath(string dataPath);
    
    /** This function loads visageSDK library and data loding scripts. 
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _preloadExternalJS(string fileName);

    /** This function starts face tracking on current frame and returns tracker status.
    * 
    * Implemented in VisageTrackerUnity library.
     */
    [DllImport("__Internal")]
    public static extern void _track();

    /** This function returns array of tracking statuses for each of the tracked faces.
     * 
     * Implemented in VisageTrackerUnity library.
     */
    [DllImport("__Internal")]
    public static extern void _getTrackerStatus(int[] tStatus, int len);

    public static void _getTrackerStatus(int[] tStatus)
    {
        _getTrackerStatus(tStatus, Tracker.MAX_FACES);
    }

    /** This function adds files to File System.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _preloadFile(string fileName);

    /** This function initialises the license.
 	 * 
 	 * Implemented in VisageTrackerUnityPlugin library.
 	 */
    [DllImport("__Internal")]
    public static extern void _initializeLicense(string license);

    /** This function grabs current frame.
     * 
     * Implemented in VisageTrackerUnity library.
     */
    [DllImport("__Internal")]
    public static extern void _grabFrame();

    /** This function binds texture ID from Unity to WebGL texture object
     * 
     * Implemented in VisageTrackerUnityPlugin library.
     */
    [DllImport("__Internal")]
    public static extern void _bindTexture(IntPtr id);

    /** This function initializes new camera.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _openCamera(int width, int height, int mirrored, string onSucessCallbackCamera, string onErrorCallbackCamera);

    /** This function closes camera.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern bool _closeCamera();

    /** This function returns camera info.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getCameraInfo(out float CameraFocus, out int ImageWidth, out int ImageHeight);
	
	/** This function returns face bounding box.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getFaceBoundingBox(ref VsRect rectangle, int faceIndex);

    /** This function returns head translation x, y and z coordinates.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getHeadTranslation(float[] translation, int faceIndex);

    /** This function returns head rotation x, y and z coordinates.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getHeadRotation(float[] rotation, int faceIndex);

    /** This function returns apparent head rotation x, y and z coordinates.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getHeadRotationApparent(float[] rotationApparent, int faceIndex);

    /** This function returns number of vertices in the 3D face model.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern int _getFaceModelVertexCount();

    /** This function returns vertex coordinates.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getFaceModelVertices(float[] vertices, int faceIndex);

    /** This function returns projected (image space) vertex coordinates.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getFaceModelVerticesProjected(float[] verticesProjected, int faceIndex);

    /** Returns number of triangles in the 3D face model. 
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern int _getFaceModelTriangleCount();

    /** This function returns triangles coordinates.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getFaceModelTriangles(int[] triangles, int faceIndex);

    /** Returns texture coordinates.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getFaceModelTextureCoords(float[] texCoord, int faceIndex);
	
	/** This functions returns static texture coordinates of the mesh.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getTexCoordsStatic(float[] buffer, out int texCoordNumber);

    /** This function returns all global 3D feature point positions.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getAllFeaturePoints3D(float[] byteOffset, int length, int faceIndex);

    /** Returns global 3D feature point positions.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getFeaturePoints3D(int number, int[] groups, int[] indices, float[] positions3D, int[] defined, int[] detected, float[] quality, int faceIndex);

    /** This function returns the feature points positions in normalized 2D screen coordinates.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getAllFeaturePoints2D(float[] byteOffset, int length, int faceIndex);

    /** This function returns the feature points positions in normalized 2D screen coordinates.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getFeaturePoints2D(int number, int[] groups, int[] indices, float[] positions3D, int[] defined, int[] detected, float[] quality, int faceIndex);

    /** This function returns the 3D coordinates relative to the face origin, placed at the center between eyes.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getAllFeaturePoints3DRelative(float[] byteOffset, int length, int faceIndex);

    /** This function returns the 3D coordinates relative to the face origin, placed at the center between eyes.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getFeaturePoints3DRelative(int number, int[] groups, int[] indices, float[] positions3D, int[] defined,  int[] detected, float[] quality, int faceIndex);

    /** This function returns index of the first feature point group.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern int _getFP_START_GROUP_INDEX();

    /** This function returns index of the last feature point group.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern int _getFP_END_GROUP_INDEX();

    /** Returns the number of feature points per group.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getGroupSizes(int[] byteOffset, int length);

    /** This function sets the inter pupillary distance. 
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _setIPD(float ipd);

    /** This function returns the current inter pupillary distance (IPD) setting. 
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern float _getIPD();

    /** This function sets tracking configuration. 
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _setTrackerConfigurationFile(string trackerConfigFile, bool au_fitting, bool mesh_fitting);

    /** This functions returns estimated tracking quality level for the current frame. The value is between 0 and 1.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern float _getTrackingQuality(int faceIndex);

    /** This function returns the frame rate of the tracker, in frames per second, measured over last 10 frames.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern float _getFrameRate();

    /** This function returns timestamp of the current video frame.
    *    
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern float _getTimeStamp();

    /** This function returns the action unit count.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern int _getActionUnitCount();

    /** This function returns scale in pixels of facial bounding box for the given faceIndex. 
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern float _getFaceScale(int faceIndex);

    /** This function returns gaze quality. 
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern float _getGazeQuality(int faceIndex);

    /** This function returns screen space gaze data. 
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getScreenSpaceGazeData(float[] ssgData, int faceIndex);

    /** This function returns eye closure values. 
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getEyeClosure(float[] closure, int faceIndex);

    /** This function returns iris radius values for the given faceIndex. 
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getIrisRadius(float[] irisRadius, int faceIndex);

    /** This function returns the name of the action unit with specified index.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern string _getActionUnitsNames(int auIndex);

    /** This function returns action units values. 
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getActionUnits(float[] values, int faceIndex);

    /** This function returns true if the action unit is used.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern bool _getActionUnitsUsed(int auIndex);

    /** This function returns global gaze direction coordinates. 
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getGazeDirectionGlobal(float[] gazeDirection, int faceIndex);

    /** This function returns gaze direction coordinates. 
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _getGazeDirection(float[] gazeDirection, int faceIndex);

    #endregion


    #region Analyser

    /** This function initialises the analyser.
    * 
    * Implemented in VisageAnalyserUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _initAnalyser(string initCallback);
    /** This function performs face analysis on a single image.
    * 
    * Implemented in VisageAnalyserUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern VFAReturnCode _analyseImage(int faceIndex, int options, float[] results);
    /** This function performs face analysis on a series of consecutive frames.
    * 
    * Implemented in VisageAnalyserUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern VFAReturnCode _analyseStream(int faceIndex, int options, float[] results);
    /** This function resets the face analysis data for all faces, or for a particular face.
    * 
    * Implemented in VisageAnalyserUnityPlugin library.
    */
    [DllImport("__Internal")]
    public static extern void _resetStreamAnalysis(int faceIndex);
    
    #endregion

#endif
}