using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MinimapRadar;

public class StampAssignment : MonoBehaviour
{
    public Image stamp;
    private ParticleSystem particles;

    private void Start()
    {
        stamp.color = Color.black;
        GetComponent<MinimapItem>().SetSprite(stamp.sprite);
        particles = transform.Find("Particle System").GetComponent<ParticleSystem>();
    }

    public void CollectStamp()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<MinimapItem>().enabled = false;
        stamp.color = Color.white;
        var main = particles.main;
        main.loop = false;
        main.stopAction = ParticleSystemStopAction.Destroy;
    }
}
