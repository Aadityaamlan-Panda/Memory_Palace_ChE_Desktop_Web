using System.Collections;
using UnityEngine;

namespace SojaExiles
{
    public class OpenCloseDoor1 : MonoBehaviour
    {
        public Animator openandclose1;
        public bool open;
        public Transform Player;

        public float interactDistance = 15f;

        void Start()
        {
            open = false;
        }

        // 🔥 CALLED FROM RAYCAST SYSTEM
        public void Interact()
        {
            if (Player == null)
            {
                Debug.LogWarning("Player not assigned!");
                return;
            }

            float dist = Vector3.Distance(Player.position, transform.position);

            if (dist > interactDistance)
            {
                Debug.Log("Too far to interact");
                return;
            }

            if (!open)
            {
                StartCoroutine(opening());
            }
            else
            {
                StartCoroutine(closing());
            }
        }

        IEnumerator opening()
        {
            Debug.Log("you are opening the door");
            openandclose1.Play("Opening 1");
            open = true;
            yield return new WaitForSeconds(0.5f);
        }

        IEnumerator closing()
        {
            Debug.Log("you are closing the door");
            openandclose1.Play("Closing 1");
            open = false;
            yield return new WaitForSeconds(0.5f);
        }
    }
}