using UnityEngine;
using System.Collections;

public class StealthClock : MonoBehaviour
{


	private int startDegree;
	private int endDegree;

	private int numberOfGuards;

	//array of empty objects to hold line renderers, one object/line for each degree
	private GameObject[] lines = new GameObject[360];

	
	private GameObject clockHand;

	// Use this for initialization
	void Start()
	{

		//initialize lines for arc
		lineSetup();

		//get size of green area
		startDegree = GetComponentInParent<StealthHand>().startDegree;
		endDegree = GetComponentInParent<StealthHand>().endDegree;

	}
	

	void lineSetup()
	{

		//create a line renderer for each object in the array
		for(int i = 0; i < lines.Length; i++)
		{

			//set the position of each game object with line renderers
			Vector3 tempPos = transform.position;
			tempPos[1] = transform.position.y + 0.1f;
			lines[i].transform.position = tempPos;

			//add a line renderer to the empty game object
			LineRenderer lRend = lines[i].AddComponent<LineRenderer>();

			//only one line per game object
			lRend.SetVertexCount(1);

			//set all lines to red
			lRend.SetColors(Color.red, Color.red);

			//do math to point lines in correct directions, find end point for each line



		}

		

	}
	// Update is called once per frame
	void Update()
	{

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
		drawArcs(GetComponentInParent<StealthHand>().startDegree, GetComponentInParent<StealthHand>().endDegree);

	}

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
}
