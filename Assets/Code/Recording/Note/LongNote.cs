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

        public void DestroyOtherHalf()
        {
            if (!otherHalf.mySelectable.isSelected) 
            {
                Destroy(otherHalf.gameObject);
            }
        }

        public void MoveLongNote()
        {
            float distance = Mathf.Abs(otherHalf.transform.position.x - transform.position.x);
            float invertedScale = (1 / transform.localScale.x);
            boxCollider.offset = new Vector2(distance / 2 * invertedScale, 0f);
            boxCollider.size = new Vector2((distance - 2 * radius) * invertedScale, 0.5f);

            mySelectable.SetLongNoteError(!IsPositionValid());
            mySelectable.UpdateVisuals();
        }

        private bool IsPositionValid()
        {
            Vector3 beginToEnd = otherHalf.transform.position - transform.position;
            if (beginToEnd.y != 0)
                return false;

            if (!(beginToEnd.x < 0 ^ isBeginNote))
                return false;

            return true;
        }
    }
}
