using UnityEngine;
using System.Collections;

public class StealthClock : MonoBehaviour
{


	public int startDegree;
	public int endDegree;
	float startWidth;
	float endWidth;

	//number of guards near player
	private int numberOfGuards;
	//length of lines that make up red/green areas
	private float lineLength;
	//degrees in a circle
	private int degrees = 360;
	//
	Vector3[] linesArr;
	//starting point of each line, above center of clock face
	public Vector3 startPos;

	//array of empty objects to hold line renderers, one object/line for each degree
	public GameObject[] lines;
	
	private GameObject clockHand;

	// Use this for initialization
	void Start()
	{

		
		//get size of green area
		//		startDegree = GetComponentInParent<StealthHand>().startDegree;
		//		endDegree = GetComponentInParent<StealthHand>().endDegree;
//TESTING
		startDegree = 500;
		endDegree = 500;
//END TESTING

		//initialize line parameters
		lineLength = transform.localScale.x / 2f;
		startPos = transform.position;
		startPos = new Vector3(startPos.x, startPos.y + 0.1f, startPos.z);
		startWidth = 0f;
		endWidth = transform.localScale.x * Mathf.PI / 360f;

		lines = new GameObject[degrees];
		linesArr = new Vector3[degrees];

		//initialize lines for arc
		lineSetup();

	}
	

	void lineSetup()
	{

		//set green and red with reduced opacity
		Color colorG = new Color(0.0f, 1.0f, 0.0f, 0.7f);
		Color colorR = new Color(1.0f, 0.0f, 0.0f, 0.7f);

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

			lRend.SetWidth(startWidth, endWidth);

			lRend.material = new Material(Shader.Find("Particles/Additive"));
//			lRend.material.SetColor("Default-Particle", colorG);
			//only one line per game object
			lRend.SetVertexCount(2);

			//set line colors accordingly
			//check start and end boundaries
			//green area fills upward from startDegree to endDegree
			if(startDegree < endDegree)
			{

				if((startDegree <= i) && (i <= endDegree))
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
				if((startDegree <= i) || (i <= endDegree))
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

			//find x and z position of end of line
			lineX = startPos.x + lineLength * Mathf.Cos(Mathf.Deg2Rad * i);
			linePos[0] = lineX;
			lineZ = startPos.z + lineLength * Mathf.Sin(Mathf.Deg2Rad * i);
			linePos[2] = lineZ;

			lRend.SetPosition(0, startPos);
			lRend.SetPosition(1, linePos);
			linesArr[i] = linePos;
			
		}
		
	}

/*
	// Update is called once per frame
	void Update()
	{

		for(int i = 0; i < lines.Length; i++)
		{

//			lines[i].GetComponent<LineRenderer>().SetPosition(0, linePoints[i]);
		}

		//set
		if(startDegree < endDegree)
		{

			for(int i = startDegree; i < endDegree; i++)
			{

				lines[i].GetComponent<LineRenderer>().SetColors(Color.green, Color.green);

			}

		}
		else
		{
			for(int i = startDegree; i < lines.Length; i++)
			{

				lines[i].GetComponent<LineRenderer>().SetColors(Color.green, Color.green);

			}

			for(int i = 0; i < endDegree; i++)
			{
				lines[i].GetComponent<LineRenderer>().SetColors(Color.green, Color.green);
			}
		}

		

		//this won't work at runtime
//		drawArcs(GetComponentInParent<StealthHand>().startDegree, GetComponentInParent<StealthHand>().endDegree);

	}
*/

}
