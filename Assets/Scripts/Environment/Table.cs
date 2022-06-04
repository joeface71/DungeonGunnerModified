
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Table : MonoBehaviour, IUseable
{
    [Tooltip("The mass of the table to control the speed that it moves when pushed")]
    [SerializeField] private float itemMass;

    private BoxCollider2D boxCollider2D;
    private Animator animator;
    private Rigidbody2D rigidBody2D;
    private bool itemUsed = false;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    public void UseItem()
    {
        if (!itemUsed)
        {
            Bounds bounds = boxCollider2D.bounds;

            Vector3 closestPointToPlayer = bounds.ClosestPoint(GameManager.Instance.GetPlayer().GetPlayerPosition());

            // player to the right
            if (closestPointToPlayer.x == bounds.max.x)
            {
                animator.SetBool(Settings.flipLeft, true);
            }
            // player to the left
            else if (closestPointToPlayer.x == bounds.min.x)
            {
                animator.SetBool(Settings.flipRight, true);
            }
            // player below
            else if (closestPointToPlayer.y == bounds.min.y)
            {
                animator.SetBool(Settings.flipUp, true);
            }
            // player above
            else
            {
                animator.SetBool(Settings.flipDown, true);
            }

            // setting layer to environment ensure the bullets will collide with the table
            gameObject.layer = LayerMask.NameToLayer("Environment");

            // reduce the mass so player can move the table
            rigidBody2D.mass = itemMass;

            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.tableFlip);

            itemUsed = true;
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(itemMass), itemMass, false);
    }
#endif
    #endregion
}
