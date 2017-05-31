using UnityEngine;
using System.Collections;

public class DisplayData : MonoBehaviour
{
    public Texture2D[] signalIcons;
    
    private int indexSignalIcons = 1;
    
    TGCConnectionController controller;

    private int poorSignal1;
    private int attention1;
    private int meditation1;
    private int blink;

    private float lowalpha;
    private float highalpha;
    private float lowbeta;
    private float highbeta;
    private float lowgamma;
    private float highgamma;
    private float theta;
    private float delta;
    void Start()
    {
        Debug.Log ("DisplayData Start()...");
        
        controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();
        Debug.Log ("Controller Found!");

        controller.UpdatePoorSignalEvent += OnUpdatePoorSignal;
        controller.UpdateAttentionEvent += OnUpdateAttention;
        controller.UpdateMeditationEvent += OnUpdateMeditation;
        controller.UpdateBlinkEvent += OnUpdateBlink;
        

        controller.UpdateLowAlphaEvent += OnUpdateLowAlpha;
        controller.UpdateHighAlphaEvent += OnUpdateHighAlpha;

        controller.UpdateLowBetaEvent += OnUpdateLowBeta;
        controller.UpdateHighBetaEvent += OnUpdateHighBeta;

        controller.UpdateLowGammaEvent += OnUpdateLowGamma;
        controller.UpdateHighGammaEvent += OnUpdateHighGamma;

        controller.UpdateThetaEvent += OnUpdateTheta;

        controller.UpdateDeltaEvent += OnUpdateDelta;

        Debug.Log ("Start() Done!");
    }
    
    void OnUpdatePoorSignal(int value){
        Debug.Log ("OnUpdatePoorSignal()...");

        poorSignal1 = value;
        if(value < 25){
              indexSignalIcons = 0;
        }else if(value >= 25 && value < 51){
              indexSignalIcons = 4;
        }else if(value >= 51 && value < 78){
              indexSignalIcons = 3;
        }else if(value >= 78 && value < 107){
              indexSignalIcons = 2;
        }else if(value >= 107){
              indexSignalIcons = 1;
        }
    }
    void OnUpdateAttention(int value){
        attention1 = value;
    }
    void OnUpdateMeditation(int value){
        meditation1 = value;
    }
    void OnUpdateBlink(int value){
        blink = value;
    }
    void OnUpdateLowAlpha(float value){
        lowalpha = value;
        Debug.Log ("OnUpdateLowAlpha() Done!");
    }
    void OnUpdateHighAlpha(float value){
        highalpha = value;
        Debug.Log ("OnUpdateHighAlpha() Done!");
    }
    void OnUpdateLowBeta(float value){
        lowbeta = value;
        Debug.Log ("OnUpdateLowBeta() Done!");
    }
    void OnUpdateHighBeta(float value){
        highbeta = value;
        Debug.Log ("OnUpdateLowBeta() Done!");
    }
    void OnUpdateLowGamma(float value){
        lowgamma = value;
        Debug.Log ("OnUpdateLowAlpha() Done!");
    }
    void OnUpdateHighGamma(float value){
        highgamma = value;
        Debug.Log ("OnUpdateLowAlpha() Done!");
    }
    void OnUpdateDelta(float value){
        delta = value;
        Debug.Log ("OnUpdateDelta() Done!");
    }
    void OnUpdateTheta(float value){
        theta = value;
        Debug.Log ("OnUpdateTheta() Done!");
    }


    void OnGUI()
    {
        //Debug.Log ("OnGui()...");

        GUILayout.BeginHorizontal();
        
        
        if (GUILayout.Button("Connect"))
        {
            controller.Connect();
        }
        if (GUILayout.Button("DisConnect"))
        {
            controller.Disconnect();
            indexSignalIcons = 1;
        }
        
        GUILayout.Space(Screen.width-250);
        GUILayout.Label(signalIcons[indexSignalIcons]);
        
        GUILayout.EndHorizontal();

        
        GUILayout.Label("PoorSignal1:" + poorSignal1);
        GUILayout.Label("Attention1:" + attention1);
        GUILayout.Label("Meditation1:" + meditation1);
        GUILayout.Label("Blink:" + blink);
        GUILayout.Label("Low Alpha:" + lowalpha);
        GUILayout.Label("High Alpha:" + highalpha);
        GUILayout.Label("Low Beta:" + lowbeta);
        GUILayout.Label("High Beta:" + highbeta);
        GUILayout.Label("Low Gamma:" + lowgamma);
        GUILayout.Label("High Gamma:" + highgamma);
        GUILayout.Label ("Theta:" + theta);
        GUILayout.Label("Delta:" + delta);

        //Debug.Log ("OnGui() done!");
    }
}