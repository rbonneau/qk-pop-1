using UnityEngine;
using System.Collections;

public class StealthClock : MonoBehaviour
{
	/*
	this class attaches to a clock face
	in order for the minigame success/fail conditions to work properly,
	the clock face y axis must be inverted so the y axis is pointing down (z rotation = 180)
	*/

	//start of green area in degrees from x axis
	private int _startDegree;
	//end of green area in degrees from x axis
	private int _endDegree;

	public float startDegree
	{
		get
		{
			return _startDegree;
		}
	}

	public float endDegree
	{
		get
		{
			return _endDegree;
		}
	}

	float startWidth;
	float endWidth;

	//number of guards near player
	private int _numberOfGuards;

	public int numberOfGuards
	{
		get
		{
			return _numberOfGuards;
		}
	}

	//length of lines that make up red/green areas
	private float lineLength;
	//degrees in a circle
	private int degrees = 360;

	//starting point of each line, above center of clock face
	public Vector3 startPos;

	//array of empty objects to hold line renderers, one object/line for each degree
	public GameObject[] lines;
	
	private GameObject clockHand;

	void Awake()
	{

		//get nearby enemies
//TEMP
		_numberOfGuards = 1;
//TEMP

		//calculate number of max successful presses

		//calculate  number of max fail presses

		//calculate size of green area

		//set start and end of green area

		//TESTING
		_startDegree = 0;
		_endDegree = 90;
		//END TESTING

	}

	// Use this for initialization
	void Start()
	{

		//initialize line parameters
		lineLength = transform.localScale.x / 2f;
		startPos = transform.position;
		startPos = new Vector3(startPos.x, startPos.y + 0.1f, startPos.z);
		startWidth = 0f;
		endWidth = transform.localScale.x * Mathf.PI / 360f;

		//array of empty gameobjects to hold a single line renderer each
		lines = new GameObject[degrees];

		//initialize lines for arc
		lineSetup();

	}
	
	void lineSetup()
	{

		//set green and red with reduced opacity
		Color colorG = new Color(0.0f, 1.0f, 0.0f, 0.9f);
		Color colorR = new Color(1.0f, 0.0f, 0.0f, 0.9f);

		//set color transparency
//		colorG.a = 0.5f;
//		colorR.a = 0.5f;

		//create a line renderer for each object in the array
		for(int i = 0; i < lines.Length; i++)
		{

			float lineX;
			float lineZ;
			Vector3 linePos;

			//create the gameObject to hold the LineRenderer
			lines[i] = new GameObject();

			//set the position of each game object with line renderers
			lines[i].transform.position = startPos;

			//add a line renderer to the empty game object
			LineRenderer lRend = lines[i].AddComponent<LineRenderer>();
			//set line widths
			lRend.SetWidth(startWidth, endWidth);
			//set line renderer material
			lRend.material = new Material(Shader.Find("Particles/Additive"));

			//only one line per game object
			lRend.SetVertexCount(2);

			//set line colors accordingly
			//check start and end boundaries
			//green area fills upward from startDegree to endDegree
			if(_startDegree < _endDegree)
			{

				if((_startDegree <= i) && (i <= _endDegree))
				{

					lRend.SetColors(colorG, colorG);
				}
				else
				{

					lRend.SetColors(Color.clear, colorR);

				}

			}
			else
			{
				if((_startDegree <= i) || (i <= _endDegree))
				{

					lRend.SetColors(Color.clear, colorG);

				}
				else
				{

					lRend.SetColors(Color.clear, colorR);

				}

			}

			//do math to point lines in correct directions, find end point for each line
			//initialize end point to current position of gameObject
			linePos = startPos;

			//find x and z position of end of line, y will remain the same
			lineX = startPos.x + lineLength * Mathf.Cos(Mathf.Deg2Rad * i);
			linePos[0] = lineX;
			lineZ = startPos.z + lineLength * Mathf.Sin(Mathf.Deg2Rad * i);
			linePos[2] = lineZ;

			//set start and end points for the line
			lRend.SetPosition(0, startPos);
			lRend.SetPosition(1, linePos);
			
		}
		
	}

/*
	// Update is called once per frame
	void Update()
	{

	}
*/

}
