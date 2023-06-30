using Pronotron.Modules;
using System.Collections;
using UnityEngine;

public class CounterTestCase : MonoBehaviour
{
    public Counter counter;

    private void Start()
    {
        counter.Set(580);
        StartCoroutine(Anim());
    }

    IEnumerator Anim()
    {
        yield return new WaitForSeconds(2);

        // Negative test, should go to zero
        counter.AddWithAnimation(-counter.Current - 20);
        yield return new WaitForSeconds(2.5f);

        counter.AddWithAnimation(1625);
        yield return new WaitForSeconds(2.5f);

        counter.AddWithAnimation(-2000);
        yield return new WaitForSeconds(2.5f);

        counter.AddWithAnimation(252380);
        yield return new WaitForSeconds(2.5f);

        counter.AddWithAnimation(200000 - counter.Current );
        yield return new WaitForSeconds(2.5f);

        counter.AddWithAnimation(-1);
        yield return new WaitForSeconds(2.5f);

        counter.AddWithAnimation(10066);
        yield return new WaitForSeconds(2.5f);
    }
}
