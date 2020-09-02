using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchControls : MonoBehaviour, IPointerDownHandler, IPointerUpHandler//, IPointerExitHandler, IPointerEnterHandler
{
	Player player;

    public enum Type
    {
        LEFT,
        RIGHT,
        JUMP
    }

    public Type type = Type.LEFT;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (type)
        {
            case Type.LEFT:
                if (!player.IsMovingRight)
                    player.MoveLeft();
                break;

            case Type.RIGHT:
                if (!player.IsMovingLeft)
                    player.MoveRight();
                break;

            case Type.JUMP:
                player.ActivateJump();
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
	{
        switch (type)
        {
            case Type.LEFT:
                player.EndLeft();
                break;

            case Type.RIGHT:
                player.EndRight();
                break;

            case Type.JUMP:
                break;
        }
	}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    switch (type)
    //    {
    //        case Type.LEFT:
    //            player.EndLeft();
    //            break;

    //        case Type.RIGHT:
    //            player.EndRight();
    //            break;

    //        case Type.JUMP:
    //            break;
    //    }
    //}

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    switch (type)
    //    {
    //        case Type.LEFT:
    //            player.MoveLeft();
    //            break;

    //        case Type.RIGHT:
    //            player.MoveRight();
    //            break;

    //        case Type.JUMP:
    //            break;
    //    }
    //}
}