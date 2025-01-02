using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;
public class JPSMangaer : SingletonMono<JPSMangaer>
{
    public IMap imap;
    public JPS _jps;
    // Start is called before the first frame update
    void Start()
    {
        imap = new MapQuad(0, 0, 45, 80, 1.0f, 1.0f);
        _jps = new JPS(imap);
        new MapToolsDrawNode(imap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
