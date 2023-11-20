using UnityEngine;

public class VideoPlaneController : MonoBehaviour
{
	public Tracker Tracker;
    //
	private float aspect;
    private float texCoordX;
    private float texCoordY;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
        if (Tracker != null && Tracker.ImageWidth != 0 && Tracker.ImageHeight != 0)
        {
            // Get mesh filter
            MeshFilter meshFilter = this.GetComponent<MeshFilter>();
            Vector2[] uv = meshFilter.mesh.uv;
            Vector2[] uv2 = meshFilter.mesh.uv2;

            // Calculate texture coordinates for the 
            // video frame part of the texture (texCoordX, texCoordY)
            //
            //   <------<TexWidth>------>
            // <ImageWidth>
            // uv[0]---uv[3]---------------
            // |          |   ^           |     ^
            // |          |   |           |     |
            // |          | <ImageHeight> |     |
            // |          |   |           |     |
            // |          |   v           |     |
            // uv[1]---uv[2]              |<TexHeight>
            // |           <texCoordX,    |     |
            // |           texCoordY,>    |     |
            // |                          |     |   
            // |                          |     v
            // ----------------------------

            // Calculate uv[2] texture coordinates
            texCoordX = Tracker.ImageWidth / (float)Tracker.TexWidth;
            texCoordY = Tracker.ImageHeight / (float)Tracker.TexHeight;

            // Apply new coordinates
            uv[1].y = texCoordY;
            uv[2].x = texCoordX;
            uv[2].y = texCoordY;
            uv[3].x = texCoordX;

            meshFilter.mesh.uv = uv;
            meshFilter.mesh.uv2 = uv2;

            // Adjust texture scale so the frame fits
            // Fixate so the the frame always fits by height
            aspect = Tracker.ImageWidth / (float)Tracker.ImageHeight;
            float yScale = 100.0f;
            float xScale = yScale * aspect;
            // NOTE: due to rotation on the VideoPlane yScale is applied on z coordinate
            
            if ((Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) && Tracker.isMirrored == 1)
                gameObject.transform.localScale = new Vector3(-xScale, 1.0f, yScale);
            else
                gameObject.transform.localScale = new Vector3(xScale, 1.0f, yScale);

        }
	}
}
