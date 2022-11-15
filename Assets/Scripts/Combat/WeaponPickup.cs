using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Combat;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        // Kılıç veya farklı bir savaş aletinde ki collider trigger olduğunda Figter scriptinde ki EquippedWeapon methoduna aldığımız Weaponu göndererek kuşandık.
        [SerializeField]  Weapon weapon = null;
        [SerializeField] private float respawnTime = 3f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                other.GetComponent<Fighter>().EquippedWeapon(weapon);
                StartCoroutine(HideForSeconds(respawnTime));
            }
        }

        IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = false; //Weapon Collider false yapıyoruz.
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow); /* Child olan weaponun aktifliğini duruma göre true ya da false dönderiyoruz.
                                                        Alındıktan sonra geçen süreye göre görünüp görünmeyeceğine karar verdiğimiz nokta. */
            }
        }
    }
}

