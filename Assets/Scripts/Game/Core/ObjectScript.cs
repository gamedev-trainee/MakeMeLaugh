namespace MakeMeLaugh
{
    public class ObjectScript : ResourceScript
    {
        public int type = 0;

        private void OnMouseDown()
        {
            ObjectManager.Instance.beginPick(this);
        }

        //private void OnMouseOver()
        //{

        //}

        private void OnMouseEnter()
        {
            ObjectManager.Instance.addPick(this);
        }

        //private void OnMouseExit()
        //{

        //}

        private void OnMouseUp()
        {
            ObjectManager.Instance.endPick();
        }
    }
}
