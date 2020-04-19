using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This script gets values from CSVReader script
// It instantiates points and particles according to values read

public class PointRenderer : MonoBehaviour {

    //********Public Variables********

    // Bools for editor options
    public bool renderPointPrefabs = true;
    public bool renderParticles =  true;
    public bool renderPrefabsWithColor = true;

    // Name of the input file, no extension
    public string inputfile;

    // Indices for columns to be assigned
    public int column1 = 0;
    public int column2 = 1;
    public int column3 = 2;
    public int column4 = 3;
    public int column5 = 4;

    // Full column names from CSV (as Dictionary Keys)
    public string xColumnName;
    public string yColumnName;
    public string zColumnName;
    public string nameColumnName;
    public string typeColumnName;
     
    // Scale of particlePoints within graph, WARNING: Does not scale with graph frame
     private float plotScale = 10;

    // Scale of the prefab particlePoints
    [Range(0.0f, 0.5f)]
    public float pointScale = 0.25f;

    // Changes size of particles generated
    [Range(0.0f, 2.0f)]
    public float particleScale = 5.0f;

    // The prefab for the data particlePoints that will be instantiated
    public GameObject PointPrefab;

    // Object which will contain instantiated prefabs in hiearchy
    public GameObject PointHolder;

    // Color for the glow around the particlePoints
    private Color GlowColor; 
    
    //********Private Variables********
        // Minimum and maximum values of columns
    private float xMin;
    private float yMin;
    private float zMin;

    private float xMax;
    private float yMax;
    private float zMax;

    private int amlCount;
    private int mpnCount;
    private int nbmCount;
    private int dsCount;
    private int tmdCount;
    private int cd34Count;
    private int nup98nCount;
    private int kmt2aCount;
    private int nopCount;
    private int nup98kCount;
    private int dekCount;
    private int runx1cCount;
    private int runx1rCount;
    private int kat6Count;
    private int cbfaCount;
    private int cbfbCount;
    private int etv6Count;
    private int fuseCount;
    private int ergCount;
    private int del5qCount;
    private int monCount;
    private int rbm15Count;

    // Number of rows
    private int rowCount;

    // List for holding data from CSV reader
    private List<Dictionary<string, object>> pointList;

    // Particle system for holding point particles
    private ParticleSystem.Particle[] particlePoints; 


    //********Methods********

    void Awake()
    {
        //Run CSV Reader
        pointList = CSVReader.Read(inputfile);
    }

    // Use this for initialization
    void Start () 
	{
       
        
        // Store dictionary keys (column names in CSV) in a list
        List<string> columnList = new List<string>(pointList[1].Keys);

        Debug.Log("There are " + columnList.Count + " columns in the CSV");

        foreach (string key in columnList)
            Debug.Log("Column name is " + key);

        // Assign column names according to index indicated in columnList
        xColumnName = columnList[column1];
        yColumnName = columnList[column2];
        zColumnName = columnList[column3];
        nameColumnName = columnList[column4];
        typeColumnName = columnList[column5];
        
        // Get maxes of each axis, using FindMaxValue method defined below
        xMax = FindMaxValue(xColumnName);
        yMax = FindMaxValue(yColumnName);
        zMax = FindMaxValue(zColumnName);

        // Get minimums of each axis, using FindMinValue method defined below
        xMin = FindMinValue(xColumnName);
        yMin = FindMinValue(yColumnName);
        zMin = FindMinValue(zColumnName);
            
        AssignLabels();

        if (renderPointPrefabs == true)
        {
            // Call PlacePoint methods defined below
            PlacePrefabPoints();
                    }

        // If statement to turn particles on and off
        if ( renderParticles == true)
        {
            // Call CreateParticles() for particle system
            CreateParticles();

            // Set particle system, for point glow- depends on CreateParticles()
            GetComponent<ParticleSystem>().SetParticles(particlePoints, particlePoints.Length);
        }
                                
    }
    
		
	// Update is called once per frame
	void Update ()
    {
        //Activate Particle System
       //GetComponent<ParticleSystem>().SetParticles(particlePoints, particlePoints.Length);

    }

    // Places the prefabs according to values read in
	private void PlacePrefabPoints()
	{
                  
        // Get count (number of rows in table)
        rowCount = pointList.Count;

        for (var i = 0; i < pointList.Count; i++)
        {

            // Set x/y/z, standardized to between 0-1
            float x = (Convert.ToSingle(pointList[i][xColumnName]) - xMin) / (xMax - xMin);
            float y = (Convert.ToSingle(pointList[i][yColumnName]) - yMin) / (yMax - yMin);
            float z = (Convert.ToSingle(pointList[i][zColumnName]) - zMin) / (zMax - zMin);
            string name = pointList[i][nameColumnName].ToString();
            string type = pointList[i][typeColumnName].ToString();


            // Create vector 3 for positioning particlePoints
			Vector3 position = new Vector3 (x, y, z) * plotScale;

			//instantiate as gameobject variable so that it can be manipulated within loop
			GameObject dataPoint = Instantiate (PointPrefab, Vector3.zero, Quaternion.identity);

            
            // Make child of PointHolder object, to keep particlePoints within container in hiearchy
            dataPoint.transform.parent = PointHolder.transform;

            // Position point at relative to parent
            dataPoint.transform.localPosition = position;

            dataPoint.transform.localScale = new Vector3(pointScale, pointScale, pointScale);

            // Converts index to string to name the point the index number
            string dataPointName = name;
            string dataPointType = type;

            // Assigns name to the prefab
            dataPoint.transform.name = dataPointName;


            if (renderPrefabsWithColor == true)
            {
                // Gets material color and sets it to a new RGB color we define
                if (type == "AML")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(1.0f,0,0, 1.0f);
                if (type == "MPN")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(0.5f,0,0, 1.0f);
                if (type == "NBM")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(0,1.0f,0, 1.0f);
                if (type == "DS")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(0,0.5f,0, 1.0f);
                if (type == "TMD")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(0,0,1.0f, 1.0f);
                if (type == "CD34_NBM")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(0,0,0.5f, 1.0f);
                if (type == "NUP98-NSD1")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(1.0f,1.0f,0, 1.0f);
                if (type == "KMT2A")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(0.5f,1.0f,0, 1.0f);
                if (type == "No.Primary.Fusion.CNV")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(1.0f,0.5f,0, 1.0f);
                if (type == "NUP98-KDM5A")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(0.5f,0.5f,0, 1.0f);
                if (type == "DEK-NUP214")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(1.0f,0,1.0f, 1.0f);
                if (type == "RUNX1-CBFA2T3")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(0.5f,0,1.0f, 1.0f);
                if (type == "RUNX1-RUNX1T1")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(1.0f,0,0.5f, 1.0f);
                if (type == "KAT6A-CREBBP")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(0.5f,0,0.5f, 1.0f);
                if (type == "CBFA2T3-GLIS2")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(0,1.0f,1.0f, 1.0f);
                if (type == "CBFB-MYH11")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(0,0.5f,1.0f, 1.0f);
                if (type == "ETV6-MNX1")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(0,1.0f,0.5f, 1.0f);
                if (type == "FUS-ERG")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(0,0.5f,0.5f, 1.0f);
                if (type == "ERG-HNRNPH1")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(0.5f,1.0f,1.0f, 1.0f);
                if (type == "del5q")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(1.0f,0.5f,1.0f, 1.0f);
                if (type == "monosomy7")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(1.0f,1.0f,0.5f, 1.0f);
                if (type == "RBM15-MKL1")
                    dataPoint.GetComponent<Renderer>().material.color = new Color(0.5f,1.0f,0.5f, 1.0f);

                // Activate emission color keyword so we can modify emission color
                dataPoint.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

                dataPoint.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(x, y, z, 1.0f));

            }
                                  						
		}

	}

    // creates particlePoints in the Particle System game object
    private void CreateParticles()
    {
        //pointList = CSVReader.Read(inputfile);

        rowCount = pointList.Count;
       // Debug.Log("Row Count is " + rowCount);

        particlePoints = new ParticleSystem.Particle[rowCount];

        for (int i = 0; i < pointList.Count; i++)
        {
            // Set x/y/z, standardized to between 0-1
            float x = (Convert.ToSingle(pointList[i][xColumnName]) - xMin) / (xMax - xMin);
            float y = (Convert.ToSingle(pointList[i][yColumnName]) - yMin) / (yMax - yMin);
            float z = (Convert.ToSingle(pointList[i][zColumnName]) - zMin) / (zMax - zMin);
            string name = pointList[i][nameColumnName].ToString();
            string type = pointList[i][typeColumnName].ToString();

            // Debug.Log("Position is " + x + y + z);

            // Set point location
			particlePoints[i].position = new Vector3(x, y, z) * plotScale;
          
            //GlowColor = 
            // Set point color
            particlePoints[i].startColor = new Color(x, y, z, 1.0f);
            particlePoints[i].startSize = particleScale; 
        }
                
    }

    // Finds labels named in scene, assigns values to their text meshes
    // WARNING: game objects need to be named within scene
    private void AssignLabels()
    {
        // find counts for each AML_Subtype
        amlCount = FindCount("AML_Subtype", "AML");
        nbmCount = FindCount("AML_Subtype", "NBM");
        dsCount = FindCount("AML_Subtype", "DS");
        tmdCount = FindCount("AML_Subtype", "TMD");
        cd34Count = FindCount("AML_Subtype", "CD34_NBM");
        nup98nCount = FindCount("AML_Subtype", "NUP98-NSD1");
        kmt2aCount = FindCount("AML_Subtype", "KMT2A");
        nopCount = FindCount("AML_Subtype", "No.Primary.Fusion.CNV");
        nup98kCount = FindCount("AML_Subtype", "NUP98-KDM5A");
        dekCount = FindCount("AML_Subtype", "DEK-NUP214");
        runx1cCount = FindCount("AML_Subtype", "RUNX1-CBFA2T3");
        runx1rCount = FindCount("AML_Subtype", "RUNX1-RUNX1T1");
        kat6Count = FindCount("AML_Subtype", "KAT6A-CREBBP");
        cbfaCount = FindCount("AML_Subtype", "CBFA2T3-GLIS2");
        cbfbCount = FindCount("AML_Subtype", "CBFB-MYH11");
        etv6Count = FindCount("AML_Subtype", "ETV6-MNX1");
        fuseCount = FindCount("AML_Subtype", "FUS-ERG");
        ergCount = FindCount("AML_Subtype", "ERG-HNRNPH1");
        del5qCount = FindCount("AML_Subtype", "del5q");
        monCount = FindCount("AML_Subtype", "monosomy7");
        rbm15Count = FindCount("AML_Subtype", "RBM15-MKL1");
        mpnCount = FindCount("AML_Subtype", "MPN");
        
        // Update total point counter
        GameObject.Find("Point_Count").GetComponent<TextMesh>().text = pointList.Count.ToString("0");

        // Update label counters
        GameObject.Find("AML_Color").GetComponent<TextMesh>().text = amlCount.ToString("0");
        GameObject.Find("AML_Color").GetComponent<Renderer>().material.color = new Color(1.0f,0,0, 1.0f);
        GameObject.Find("NBM_Color").GetComponent<TextMesh>().text = nbmCount.ToString("0");
        GameObject.Find("NBM_Color").GetComponent<Renderer>().material.color = new Color(0,1.0f,0, 1.0f);
        GameObject.Find("DS_Color").GetComponent<TextMesh>().text = dsCount.ToString("0");
        GameObject.Find("DS_Color").GetComponent<Renderer>().material.color = new Color(0,0.5f,0, 1.0f);
        GameObject.Find("TMD_Color").GetComponent<TextMesh>().text = tmdCount.ToString("0");
        GameObject.Find("TMD_Color").GetComponent<Renderer>().material.color = new Color(0,0,1.0f, 1.0f);
        GameObject.Find("CD34_Color").GetComponent<TextMesh>().text = cd34Count.ToString("0");
        GameObject.Find("CD34_Color").GetComponent<Renderer>().material.color = new Color(0,0,0.5f, 1.0f);
        GameObject.Find("NUP98N_Color").GetComponent<TextMesh>().text = nup98nCount.ToString("0");
        GameObject.Find("NUP98N_Color").GetComponent<Renderer>().material.color = new Color(1.0f,1.0f,0, 1.0f);
        GameObject.Find("KMT2A_Color").GetComponent<TextMesh>().text = kmt2aCount.ToString("0");
        GameObject.Find("KMT2A_Color").GetComponent<Renderer>().material.color = new Color(0.5f,1.0f,0, 1.0f);
        GameObject.Find("NoP_Color").GetComponent<TextMesh>().text = nopCount.ToString("0");
        GameObject.Find("NoP_Color").GetComponent<Renderer>().material.color = new Color(1.0f,0.5f,0, 1.0f);
        GameObject.Find("NUP98K_Color").GetComponent<TextMesh>().text = nup98kCount.ToString("0");
        GameObject.Find("NUP98K_Color").GetComponent<Renderer>().material.color = new Color(0.5f,0.5f,0, 1.0f);
        GameObject.Find("DEK_Color").GetComponent<TextMesh>().text = dekCount.ToString("0");
        GameObject.Find("DEK_Color").GetComponent<Renderer>().material.color = new Color(1.0f,0,1.0f, 1.0f);
        GameObject.Find("RUNX1C_Color").GetComponent<TextMesh>().text = runx1cCount.ToString("0");
        GameObject.Find("RUNX1C_Color").GetComponent<Renderer>().material.color = new Color(0.5f,0,1.0f, 1.0f);
        GameObject.Find("RUNX1R_Color").GetComponent<TextMesh>().text = runx1rCount.ToString("0");
        GameObject.Find("RUNX1R_Color").GetComponent<Renderer>().material.color = new Color(1.0f,0,0.5f, 1.0f);
        GameObject.Find("KAT6_Color").GetComponent<TextMesh>().text = kat6Count.ToString("0");
        GameObject.Find("KAT6_Color").GetComponent<Renderer>().material.color = new Color(0.5f,0,0.5f, 1.0f);
        GameObject.Find("CBFA_Color").GetComponent<TextMesh>().text = cbfaCount.ToString("0");
        GameObject.Find("CBFA_Color").GetComponent<Renderer>().material.color = new Color(0,1.0f,1.0f, 1.0f);
        GameObject.Find("CBFB_Color").GetComponent<TextMesh>().text = cbfbCount.ToString("0");
        GameObject.Find("CBFB_Color").GetComponent<Renderer>().material.color = new Color(0,0.5f,1.0f, 1.0f);
        GameObject.Find("ETV6_Color").GetComponent<TextMesh>().text = etv6Count.ToString("0");
        GameObject.Find("ETV6_Color").GetComponent<Renderer>().material.color = new Color(0,1.0f,0.5f, 1.0f);
        GameObject.Find("FUSE_Color").GetComponent<TextMesh>().text = fuseCount.ToString("0");
        GameObject.Find("FUSE_Color").GetComponent<Renderer>().material.color = new Color(0,0.5f,0.5f, 1.0f);
        GameObject.Find("ERG_Color").GetComponent<TextMesh>().text = ergCount.ToString("0");
        GameObject.Find("ERG_Color").GetComponent<Renderer>().material.color = new Color(0.5f,1.0f,1.0f, 1.0f);
        GameObject.Find("del5q_Color").GetComponent<TextMesh>().text = del5qCount.ToString("0");
        GameObject.Find("del5q_Color").GetComponent<Renderer>().material.color = new Color(1.0f,0.5f,1.0f, 1.0f);
        GameObject.Find("mon_Color").GetComponent<TextMesh>().text = monCount.ToString("0");
        GameObject.Find("mon_Color").GetComponent<Renderer>().material.color = new Color(1.0f,1.0f,0.5f, 1.0f);
        GameObject.Find("RBM15_Color").GetComponent<TextMesh>().text = rbm15Count.ToString("0");
        GameObject.Find("RBM15_Color").GetComponent<Renderer>().material.color = new Color(0.5f,1.0f,0.5f, 1.0f);
        GameObject.Find("MPN_Color").GetComponent<TextMesh>().text = mpnCount.ToString("0");
        GameObject.Find("MPN_Color").GetComponent<Renderer>().material.color = new Color(0.5f,0,0, 1.0f);

        
        // Update title according to inputfile name
        GameObject.Find("Dataset_Label").GetComponent<TextMesh>().text = inputfile;

        // Update axis titles to ColumnNames
        GameObject.Find("X_Title").GetComponent<TextMesh>().text = xColumnName;
        GameObject.Find("Y_Title").GetComponent<TextMesh>().text = yColumnName;
        GameObject.Find("Z_Title").GetComponent<TextMesh>().text = zColumnName;

        // Set x Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        GameObject.Find("X_Min_Lab").GetComponent<TextMesh>().text = xMin.ToString("0.0");
        GameObject.Find("X_Mid_Lab").GetComponent<TextMesh>().text = (xMin + (xMax - xMin) / 2f).ToString("0.0");
        GameObject.Find("X_Max_Lab").GetComponent<TextMesh>().text = xMax.ToString("0.0");

        // Set y Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        GameObject.Find("Y_Min_Lab").GetComponent<TextMesh>().text = yMin.ToString("0.0");
        GameObject.Find("Y_Mid_Lab").GetComponent<TextMesh>().text = (yMin + (yMax - yMin) / 2f).ToString("0.0");
        GameObject.Find("Y_Max_Lab").GetComponent<TextMesh>().text = yMax.ToString("0.0");

        // Set z Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        GameObject.Find("Z_Min_Lab").GetComponent<TextMesh>().text = zMin.ToString("0.0");
        GameObject.Find("Z_Mid_Lab").GetComponent<TextMesh>().text = (zMin + (zMax - zMin) / 2f).ToString("0.0");
        GameObject.Find("Z_Max_Lab").GetComponent<TextMesh>().text = zMax.ToString("0.0");
                
    }

    //Method for finding max value, assumes PointList is generated
    private float FindMaxValue(string columnName)
    {
        //set initial value to first value
        float maxValue = Convert.ToSingle(pointList[0][columnName]);

        //Loop through Dictionary, overwrite existing maxValue if new value is larger
        for (var i = 0; i < pointList.Count; i++)
        {
            if (maxValue < Convert.ToSingle(pointList[i][columnName]))
                maxValue = Convert.ToSingle(pointList[i][columnName]);
        }

        //Spit out the max value
        return maxValue;
    }

    //Method for finding minimum value, assumes PointList is generated
    private float FindMinValue(string columnName)
    {
        //set initial value to first value
        float minValue = Convert.ToSingle(pointList[0][columnName]);

        //Loop through Dictionary, overwrite existing minValue if new value is smaller
        for (var i = 0; i < pointList.Count; i++)
        {
            if (Convert.ToSingle(pointList[i][columnName]) < minValue)
                minValue = Convert.ToSingle(pointList[i][columnName]);
        }

        return minValue;
    }

    //Method for finding type counts
    private int FindCount(string columnName, string t)
    {
        //set initial value to first value
        int res = 0;

        //Loop through Dictionary, overwrite existing maxValue if new value is larger
        for (var i = 0; i < pointList.Count; i++)
        {
            if (pointList[i][columnName].ToString() == t)
                res++;
        }

        //Spit out the total count
        return res;
    }

}


