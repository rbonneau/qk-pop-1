using UnityEngine;
using System.Collections;

public class StealthClock : MonoBehaviour
{


	public int startDegree;
	public int endDegree;

	private int numberOfGuards;

	private float lineLength;

	private int degrees = 360;

	public Vector3 tempPos;
	Vector3[] linePoints;
	public Vector3 startPos;

	//array of empty objects to hold line renderers, one object/line for each degree
	public GameObject[] lines;// = new GameObject[360];

	
	private GameObject clockHand;


	

	// Use this for initialization
	void Start()
	{

		lineLength = transform.localScale.x;
		startPos = transform.position;
		startPos = new Vector3(startPos.x, startPos.y + 0.1f, startPos.z);

		lines = new GameObject[degrees];
		linePoints = new Vector3[degrees];

		//initialize lines for arc
		lineSetup();

		//get size of green area
		//		startDegree = GetComponentInParent<StealthHand>().startDegree;
		//		endDegree = GetComponentInParent<StealthHand>().endDegree;

		startDegree = 0;
		endDegree = 5;

	}
	

	void lineSetup()
	{

		Color colorG = new Color(0.0f, 1.0f, 0.0f, 0.5f);
		Color colorR = new Color(1.0f, 0.0f, 0.0f, 0.5f);

		//set color transparency
		colorG.a = 0.5f;
		colorR.a = 0.5f;

		//create a line renderer for each object in the array
		for(int i = 0; i < lines.Length; i++)
		{

			float lineX;
			float lineZ;
			Vector3 linePos;

			//create the gameObject to hold the LineRenderer
			lines[i] = new GameObject();

			//set the position of each game object with line renderers
//			tempPos = transform.position;
//			tempPos[1] = transform.position.y + 0.1f;
			tempPos = startPos;
			tempPos[1] = startPos.y + 0.1f;
			lines[i].transform.position = tempPos;

			//add a line renderer to the empty game object
			LineRenderer lRend = lines[i].AddComponent<LineRenderer>();

			lRend.material.SetColor("Default-Particle", colorG);
			//only one line per game object
			lRend.SetVertexCount(2);

			//set line colors accordingly
			//check start and end boundaries
			//green area fills upward from startDegree to endDegree
			if(startDegree < endDegree)
			{

				if((startDegree <= i) && (i <= endDegree))
				{

//					lRend.SetColors(colorG, colorG);
//					lRend.material.SetColor("Default-Particle", colorG);
					lRend.SetColors(Color.clear, colorG);

				}
				else
				{

//					lRend.SetColors(colorR, colorR);
//					lRend.material.SetColor("Default-Particle", colorR);
					lRend.SetColors(Color.clear, colorR);

				}

			}
			else
			{
				if((startDegree <= i) && (i <= endDegree))
				{

//					lRend.SetColors(colorR, colorR);
//					lRend.material.SetColor("Default-Particle", colorR);
					lRend.SetColors(Color.clear, colorR);

				}
				else
				{

//					lRend.SetColors(colorG, colorG);
//					lRend.material.SetColor("Default-Particle", colorG);
					lRend.SetColors(Color.clear, colorG);

				}

			}

			//do math to point lines in correct directions, find end point for each line
			//initialize end point to current position of gameObject
			linePos = tempPos;

			//find x and z position of end of line
			lineX = startPos.x + lineLength * Mathf.Cos(i * 180.0f / Mathf.PI);// * (180.0f / Mathf.PI);// / 100f;
			linePos[0] = lineX;
			lineZ = startPos.z + lineLength * Mathf.Sin(i * 180.0f / Mathf.PI);// * (180.0f / Mathf.PI); // 100f;
			linePos[2] = lineZ;

			lRend.SetPosition(0, startPos);
			lRend.SetPosition(1, linePos);
			linePoints[i] = linePos;
			
		}
		
	}
	// Update is called once per frame
	void Update()
	{

		for(int i = 0; i < lines.Length; i++)
		{

//			lines[i].GetComponent<LineRenderer>().SetPosition(0, linePoints[i]);
		}
/*
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
*/
		

		//this won't work at runtime
//		drawArcs(GetComponentInParent<StealthHand>().startDegree, GetComponentInParent<StealthHand>().endDegree);

	}

/*
	void drawArcs(int start, int end)
	{

		start = start % 360;
		end = end % 360;

		drawArc(start, end, Color.green);
		drawArc(end, start, Color.red);

	}

	void drawArc(int start, int end, Color color)
	{
		
		//draw color solid arc from start to end
		//have to use line renderer

	}
*/

}
