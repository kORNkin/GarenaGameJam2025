using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseController : MonoBehaviour
{
    [SerializeField] GameController gameController;

    bool bluePackageRemain = false, redPackageRemain = false;

    public GameObject bluePackagePrefab;
    public GameObject redPackagePrefab;

    public GameObject parentPackage;
    
    GameObject bluePackage;
    GameObject redPackage;
    
    public bool GetPackageRemain(string str)
    {
        if(str == "blue") return bluePackageRemain;
        else if(str == "red") return redPackageRemain;
        return false;
    }

    public bool SpawnBluePackage()
    {
        if (bluePackageRemain) return false;

        bluePackageRemain = true;

        bluePackage = Instantiate(bluePackagePrefab, parentPackage.transform);

        return true;
    }
    public bool SpawnRedPackage()
    {
        if (redPackageRemain) return false;

        redPackageRemain = true;

        redPackage = Instantiate(redPackagePrefab, parentPackage.transform);

        return true;
    }
    public void ReceiveBluePackage()
    {
        StartCoroutine(ReceiveBluePackageAnimation());
    }
    public void ReceiveRedPackage()
    {
        StartCoroutine(ReceiveRedPackageAnimation());
    }

    IEnumerator ReceiveBluePackageAnimation()
    {
        bluePackage.GetComponent<Animator>().SetTrigger("receive");
        yield return new WaitWhile(() => bluePackage.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("receive"));

        bluePackageRemain = false;

        Destroy(bluePackage);
    }

    IEnumerator ReceiveRedPackageAnimation()
    {
        redPackage.GetComponent<Animator>().SetTrigger("receive");
        yield return new WaitWhile(() => redPackage.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("receive"));

        redPackageRemain = false;

        Destroy(redPackage);
    }
}
