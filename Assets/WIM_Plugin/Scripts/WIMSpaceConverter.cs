using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface WIMSpaceConverter {
    Vector3 ConvertToLevelSpace(Vector3 pointInWIMSpace);

    Vector3 ConvertToWIMSpace(Vector3 pointInLevelSpace);
}