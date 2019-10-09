using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using WIM_Plugin;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OVRGrabbable))]
[RequireComponent(typeof(DistanceGrabbable))]
public class MiniatureModel : MonoBehaviour {
    public WIMConfiguration Configuration;
    public WIMData Data;
    public readonly WIMGenerator Generator;
    public WIMSpaceConverter Converter;

    public delegate void WIMAction(WIMConfiguration config, WIMData data);
    public static event WIMAction OnStart;
    public static event WIMAction OnUpdate;
    public static event WIMAction OnNewDestinationSelected;
    public static event WIMAction OnPreTravel;
    public static event WIMAction OnPostTravel;

    private TravelStrategy travelStrategy;


    public MiniatureModel() {
        Generator = new WIMGenerator();
    }

    void Awake() {
        if(!ConfigurationIsThere()) return;
        Data = new WIMData();
        Converter = new WIMSpaceConverterImpl(Configuration, Data);
        travelStrategy = new InstantTravel();
        Data.WIMLevelTransform = GameObject.Find("WIM Level")?.transform;
        Data.LevelTransform = GameObject.Find("Level")?.transform;
        Data.PlayerTransform = GameObject.Find("OVRCameraRig")?.transform;
        Data.HMDTransform = GameObject.Find("CenterEyeAnchor")?.transform;
        Data.fingertipIndexR = GameObject.Find("hands:b_r_index_ignore")?.transform;
        Data.OVRPlayerController = GameObject.Find("OVRPlayerController")?.transform;
        Assert.IsNotNull(Data.WIMLevelTransform);
        Assert.IsNotNull(Data.HMDTransform);
        Assert.IsNotNull(Data.fingertipIndexR);
        Assert.IsNotNull(Configuration.PlayerRepresentation);
        Assert.IsNotNull(Configuration.DestinationIndicator);
        Assert.IsNotNull(Data.OVRPlayerController);
        Assert.IsNotNull(Data.LevelTransform);
        Assert.IsNotNull(Data.PlayerTransform);
    }

    void Start() {
        if(!ConfigurationIsThere()) return;
        OnStart?.Invoke(Configuration, Data);
    }

    void Update() {
        if(!ConfigurationIsThere()) return;
        OnUpdate?.Invoke(Configuration, Data);
    }

    private bool ConfigurationIsThere() {
        if(Configuration) return true;
        Debug.LogError("WIM configuration missing.");
        return false;
    }

    public void NewDestination() {
        OnNewDestinationSelected?.Invoke(Configuration, Data);
    }

    public void ConfirmTravel() {
        DestinationIndicators.RemoveDestinationIndicators(this);
        OnPreTravel?.Invoke(Configuration, Data);
        travelStrategy.Travel(this);
        OnPostTravel?.Invoke(Configuration, Data);
    }
}