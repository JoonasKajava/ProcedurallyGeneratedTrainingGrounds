using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// This is generic widget to display information about jobs using images or text (terrain generation, ai vision, ai training data) 
public class JobMonitor : MonoBehaviour
{
    #region Variables

    [Tooltip("Title displayed on top of the information")]
    private string _title = "";
    public string Title {
        get
        {
            return _title;
        }
        set
        {
            if (_title == value) return;
            _title = value;
            if (OnTitleChange != null) OnTitleChange(value);
        }
    }

    // Wrapper object for multiple jobmonitors
    private GameObject JobMonitorsGameObject;
    private Canvas JobCanvas;

    // Text
    private GameObject JobTitleGameObject;
    private Text JobTitle;
    private ContentSizeFitter JobTitleTextFitter;
    private RectTransform TextRectTransform;

    // Image
    private GameObject JobImageGameObject;
    private RawImage JobImage;
    private Color[,] _imageArray;
    public Color[,] ImageArray
    {
        get
        {
            return _imageArray;
        }set
        {
            if (_imageArray == value) return;
            _imageArray = value;
            if (OnImageArrayChange != null) OnImageArrayChange(value);
        }
    }

    private static int JobMonitorCount = 0;

    #endregion

    #region Events
    public delegate void OnTitleChangeDelegate(string title);
    public event OnTitleChangeDelegate OnTitleChange;

    public delegate void OnImageArrayChangeDelegate(Color[,] colors);
    public event OnImageArrayChangeDelegate OnImageArrayChange;

    #endregion


    public static JobMonitor CreateInstance()
    {
        return new GameObject().AddComponent<JobMonitor>();
    }

    // Start is called before the first frame update
    void Start()
    {
        #region Initialization
        OnTitleChange += TitleChanged;
        OnImageArrayChange += ImageArrayChanged;

        if (GameObject.FindGameObjectsWithTag("JobMonitor").Length < 1)
        {
            JobMonitorsGameObject = new GameObject();
            JobMonitorsGameObject.name = "JobMonitors";
            // Canvas
            JobCanvas = JobMonitorsGameObject.AddComponent<Canvas>();
            JobCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            JobMonitorsGameObject.AddComponent<CanvasScaler>();
            JobMonitorsGameObject.AddComponent<GraphicRaycaster>();
            var GridLayout = JobMonitorsGameObject.AddComponent<GridLayoutGroup>();
            GridLayout.cellSize = new Vector2(150, 150);
            GridLayout.padding = new RectOffset(15, 0, 15, 0);
            GridLayout.spacing = new Vector2(15, 15);

        }else
        {
            JobMonitorsGameObject = GameObject.Find("JobMonitors");
        }

        gameObject.name = "JobMonitor " + JobMonitorCount;
        gameObject.AddComponent<Canvas>();
        var VerticalLayoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
        VerticalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        VerticalLayoutGroup.childControlWidth = true;
        VerticalLayoutGroup.childControlHeight = false;
        
        gameObject.tag = "JobMonitor";
        gameObject.transform.parent = JobMonitorsGameObject.transform;
        JobMonitorCount++;


        // Title
        JobTitleGameObject = new GameObject();
        JobTitleGameObject.transform.parent = gameObject.transform;

        JobTitle = JobTitleGameObject.AddComponent<Text>();
        JobTitleTextFitter = JobTitleGameObject.AddComponent<ContentSizeFitter>();
        JobTitleTextFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        JobTitleTextFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        JobTitle.name = "JobMonitorTitle";
        JobTitle.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        JobTitle.text = Title ?? JobTitleGameObject.name;

        TextRectTransform = JobTitleGameObject.GetComponent<RectTransform>();
        TextRectTransform.localPosition = new Vector3(0, 0, 0);
        TextRectTransform.anchoredPosition = new Vector2(0.5f, 0.5f);
        TextRectTransform.anchorMin = new Vector2(0.5f, 1f);
        TextRectTransform.anchorMax = new Vector2(0.5f, 1f);
        TextRectTransform.pivot = new Vector2(0.5f, 0.5f);
        TextRectTransform.position = new Vector3(0f, 25f, 0f);


        // Image
        JobImageGameObject = new GameObject();
        JobImageGameObject.AddComponent<RectTransform>().sizeDelta = new Vector2(150, 150);
        JobImageGameObject.transform.parent = gameObject.transform;
        JobImage = JobImageGameObject.AddComponent<RawImage>();
        JobImage.texture = new Texture2D(150, 150, TextureFormat.ARGB32, false);
        if(ImageArray != null)
        {
            ImageArrayChanged(ImageArray);
        }
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TitleChanged(string title)
    {
        JobTitle.text = title ?? JobTitleGameObject.name;
    }

    private void ImageArrayChanged(Color[,] colors)
    {

        var NewTexture = new Texture2D(colors.GetLength(0), colors.GetLength(1));

        NewTexture.SetPixels(colors.ToFlat());
        NewTexture.Apply();

        JobImage.texture = NewTexture;
    }

    private void OnValidate()
    {
        Title = _title;
    }

}
