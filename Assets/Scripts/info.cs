using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class info : MonoBehaviour {
    public int id;
    public Wire wire;
    void Start() {
        id = wire.id;
    }
}