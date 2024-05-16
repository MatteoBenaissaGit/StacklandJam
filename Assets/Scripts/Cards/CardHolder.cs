using UnityEngine;

namespace Cards
{
    public abstract class CardHolder : MonoBehaviour
    {
        public abstract void HoldCard(CardController card);
    }
}
