using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Recording.Note
{
    public class LongNote : MonoBehaviour
    {
        [SerializeField]
        private LongNote otherHalf;

        [SerializeField]
        private BoxCollider2D boxCollider;

        private Selectable mySelectable;
        private float radius;

        private bool isBeginNote = false;


        private void OnEnable()
        {
            radius = GetComponent<CircleCollider2D>().radius;
            mySelectable = GetComponent<Selectable>();
        }

        public void SetOtherHalf(GameObject otherHalf)
        {
            this.otherHalf = otherHalf.transform.GetComponent<LongNote>();
        }

        private void SetBoxCollider(BoxCollider2D refBoxCollider)
        {
            boxCollider = refBoxCollider;
        }

        public void InitializePair()
        {
            BoxCollider2D refBoxCollider = transform.AddComponent<BoxCollider2D>();
            float distance = Mathf.Abs(otherHalf.transform.position.x - transform.position.x);
            float invertedScale = (1 / transform.localScale.x);
            refBoxCollider.offset = new Vector2(distance / 2 * invertedScale, 0f);
            refBoxCollider.size = new Vector2((distance - 2 * radius) * invertedScale, 0.5f);
            refBoxCollider.isTrigger = true;

            SetBoxCollider(refBoxCollider);
            otherHalf.SetBoxCollider(refBoxCollider);

            isBeginNote = true;
            otherHalf.isBeginNote = false;
        }

        public GameObject GetOtherHalf()
        {
            return otherHalf.transform.gameObject;
        }

        public void MoveLongNote()
        {
            float distance = Mathf.Abs(otherHalf.transform.position.x - transform.position.x);
            float invertedScale = (1 / transform.localScale.x);
            boxCollider.offset = new Vector2(distance / 2 * invertedScale, 0f);
            boxCollider.size = new Vector2((distance - 2 * radius) * invertedScale, 0.5f);

            mySelectable.SetLongNoteError(!IsPositionValid());
            mySelectable.UpdateVisuals();

            UpdateNoteConnector();
        }

        private bool IsPositionValid()
        {
            Vector3 beginToEnd = otherHalf.transform.position - transform.position;
            if (beginToEnd.y != 0)
                return false;

            if (!(beginToEnd.x < 0 ^ isBeginNote))
                return false;

            if (IsNoteBetween())
                return false;

            return true;
        }

        private bool IsNoteBetween()
        {
            Collider2D[] colliders = new Collider2D[10];

            int collidersCount = boxCollider.OverlapCollider(new ContactFilter2D().NoFilter(), colliders);

            if (collidersCount == 0) 
                return false;

            for (int i = 0; i < collidersCount; i++)
            {
                if (colliders[i].CompareTag("Note"))
                    return true;
            }

            return false;
        }

        private void UpdateNoteConnector()
        {
            LongNoteConnector longNoteConnector = transform.GetComponent<LongNoteConnector>();
            
            if (longNoteConnector == null)
                longNoteConnector = otherHalf.GetComponent<LongNoteConnector>();
            longNoteConnector.UpdateLine(0f);
        }
    }
}
