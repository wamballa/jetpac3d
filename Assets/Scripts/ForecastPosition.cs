using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForecastPosition : MonoBehaviour
{

    public enum Direction
    {
        up,
        down,
        left,
        right
    }
    public Direction direction = Direction.right;

    Quaternion upDirection = Quaternion.Euler(0, 0, 0);
    Quaternion downDirection = Quaternion.Euler(0, 180, 0);
    Quaternion leftDirection = Quaternion.Euler(0, -90, 0);
    Quaternion rightDirection = Quaternion.Euler(0, 90, 0);
    //Quaternion downDirection = new Quaternion(0f, 1f, 0f, 0f);

    public enum BlockType
    {
        none,
        upRight,
        upLeft,
        downRight,
        downLeft,
        leftUpDown,
        rightUpDown,
        leftRightDown,
        leftRightUp,
        cross
    }
    public BlockType nextBlockName;

    float dist = 1f;

    public GameObject GetForwardBlock()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out hit, dist))
        {
            return (hit.transform.gameObject);
        }
        // player facing off screen so no forward block
        return null;
    }

    public Vector3 GetForwardPos()
    {
        Vector3 newPos = transform.position + transform.forward;
        return newPos;
    }

    BlockType GetBlockName(GameObject tile)
    {
        string blockName = tile.transform.tag;

        switch (blockName)
        {
            // DECISION BLOCKS ///////////////////////////////////////
            case "LeftUpDown":
                return BlockType.leftUpDown;
                break;
            case "RightUpDown":
                return BlockType.rightUpDown;
                break;
            case "LeftRightDown":
                return BlockType.leftRightDown;
                break;
            case "LeftRightUp":
                return BlockType.leftRightUp;
                break;
            case "Cross":
                return BlockType.cross;
                break;
            // DECISION BLOCKS ///////////////////////////////////////
            case "DownLeft":
                return BlockType.downLeft;
                break;
            case "UpRight":
                return BlockType.upRight;
                break;
            case "UpLeft":
                return BlockType.upLeft;
                break;
            case "DownRight":
                return BlockType.downRight;
                break;
            case "NA":
                return BlockType.none;
                break;
            default:
                //print("ERROR GetTileType");
                return BlockType.none;
                break;
        }

    }

    bool AngleCompare(Quaternion a, Quaternion b)
    {
        float angle = Quaternion.Angle(a, b);
        return Mathf.Abs(angle) < 1e-3f;
    }
    public void HandleRotationsBasedOnPlayerPosition(GameObject nextBlock)
    {

        //Quaternion rotation = transform.eulerAngles;
        //Quaternion rotation = transform.parent.localRotation;
        //Quaternion rotation = transform.eulerAngles;

        //Vector3 temp = transform.eulerAngles;
        //temp.y = Mathf.Ceil(temp.y);
        //transform.eulerAngles = temp;

        nextBlockName = GetBlockName(nextBlock);

        if (nextBlockName == null)
        {
            print("ERROR");
            return;
        }

        // STANDARD BLOCKS
        if (nextBlockName == BlockType.upRight)
        {
            if (transform.eulerAngles == leftDirection.eulerAngles)
            {
                transform.eulerAngles = upDirection.eulerAngles;
            }
            else if (transform.eulerAngles == downDirection.eulerAngles)
            {
                transform.eulerAngles = rightDirection.eulerAngles;
            }
        }
        /////////////////////////////////
        if (nextBlockName == BlockType.upLeft)
        {
            //print("up left GLOBAL" + transform.eulerAngles + "  " + downDirection.eulerAngles);

            if (transform.eulerAngles == rightDirection.eulerAngles)
            {
                transform.eulerAngles = upDirection.eulerAngles;
            }
            else if (transform.eulerAngles == downDirection.eulerAngles)
            {
                transform.eulerAngles = leftDirection.eulerAngles;
            }
            //else if (AngleCompare(transform.eulerAngles, downDirection.eulerAngles))
            //{
            //    print("coming down");
            //    transform.eulerAngles = leftDirection.eulerAngles;
            //}
        }

        if (nextBlockName == BlockType.downLeft)
        {
            if (Quaternion.Angle(transform.rotation, rightDirection) == 0)
            {
                transform.eulerAngles = downDirection.eulerAngles;
            }
            else if (Quaternion.Angle(transform.rotation, upDirection) == 0)
            {
                transform.eulerAngles = leftDirection.eulerAngles;
            }
        }

        if (nextBlockName == BlockType.downRight)
        {
            if (Quaternion.Angle(transform.rotation, leftDirection) == 0)
            {
                transform.eulerAngles = downDirection.eulerAngles;
            }
            if (Quaternion.Angle(transform.rotation, upDirection) == 0)
            {
                transform.eulerAngles = rightDirection.eulerAngles;
            }
        }

        // DECISION BLOCKs
        // ┤ done
        // ╞ done
        // ╤ done
        // ╨  done
        // ╬ done

        // DECISION BLOCK - LEFT UP DOWN ┤
        if (nextBlockName == BlockType.leftUpDown)
        {
            // FROM RIGHT ┤
            if (transform.eulerAngles == rightDirection.eulerAngles)
            {
                if (Random.Range(1, 10) < 5)
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
            }
            // COMING UP ┤
            else if (Quaternion.Angle(transform.rotation, upDirection)==0)
            {
                if (Random.Range(1, 10) < 5)
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
            }
            // COMING DOWN ┤
            else if (transform.eulerAngles == downDirection.eulerAngles)
            {
                if (Random.Range(1, 10) < 5)
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
            }
        }

        // DECISION BLOCK - RIGHT LEFT DOWN ╤
        else if (nextBlockName == BlockType.leftRightDown)
        {
            // COMING RIGHT ╤
            if (transform.eulerAngles == rightDirection.eulerAngles)
            {
                if (Random.Range(1, 10) < 5)
                {
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
            }
            // COMING UP ╤
            else if (transform.eulerAngles == upDirection.eulerAngles)
            {
                if (Random.Range(1, 10) < 5)
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
            }
            // COMING LEFT ╤
            else if (transform.eulerAngles == leftDirection.eulerAngles)
            {
                if (Random.Range(1, 10) < 5)
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
            }
        }

        // DECISION BLOCK - RIGHT UP DOWN ╞ 
        else if (nextBlockName == BlockType.rightUpDown)
        {
            //print("coming up>>>>>    "+ transform.eulerAngles+ " "+ upDirection.eulerAngles);
            // Coming Up ╞ 
            if (transform.eulerAngles == upDirection.eulerAngles)
            {
                if (Random.Range(1, 10) < 5)
                {
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
            }
            // Coming down ╞
            else if (transform.eulerAngles == downDirection.eulerAngles)
            {
                if (Random.Range(1, 10) < 5)
                {
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
            }
            // Coming left ╞
            else if (transform.eulerAngles == leftDirection.eulerAngles)
            {
                if (Random.Range(1, 10) < 5)
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
            }
        }

        // DECISION BLOCK - LEFT RIGHT UP ╨
        else if (nextBlockName == BlockType.leftRightUp)
        {
            //print("Euler angles "+transform.eulerAngles+ " "+ leftDirection.eulerAngles.eulerAngles);
            //print("Angles " + transform.eulerAngles + " " + leftDirection.eulerAngles);
            //Vector3 temp = transform.eulerAngles;
            //temp.y = Mathf.RoundToInt(temp.y);
            //transform.eulerAngles = temp;
            //print("Round " + transform.eulerAngles + " " + leftDirection.eulerAngles.eulerAngles);
            //if (transform.eulerAngles == leftDirection.eulerAngles.eulerAngles) print("MATCH");
            // Coming down ╨
            if (transform.eulerAngles == downDirection.eulerAngles)
            {
                if (Random.Range(1, 10) < 5)
                {
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
            }
            // Coming down ╨
            else if (transform.eulerAngles == leftDirection.eulerAngles)
            {

                if (Random.Range(1, 10) < 5)
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
            }
            // Coming right ╨
            else if (transform.eulerAngles == rightDirection.eulerAngles)
            {
                if (Random.Range(1, 10) < 5)
                {
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
            }

        }

        // DECISION BLOCK - CROSS ╬
        else if (nextBlockName == BlockType.cross)
        {
            // Coming down ╬
            if (transform.eulerAngles == downDirection.eulerAngles)
            {
                if (Random.Range(1, 10) < 5)
                {
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
            }
            // Coming up ╬
            else if (transform.eulerAngles == upDirection.eulerAngles)
            {
                if (Random.Range(1, 10) < 5)
                {
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
            }
            // Coming left ╬
            else if (transform.eulerAngles == leftDirection.eulerAngles)
            {
                if (Random.Range(1, 10) < 5)
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
            }
            // Coming right ╬
            else if (transform.eulerAngles == rightDirection.eulerAngles)
            {
                if (Random.Range(1, 10) < 5)
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
            }

        }
    }

    public Vector3 MoveAlongPath()
    {
        Vector3 newPos = transform.position + transform.forward;
        Debug.DrawLine(transform.position, newPos, Color.white, 2f);
        transform.position = newPos;
        return newPos;
    }

    private void MoveAlongPath(GameObject nextBlock)
    {
        Debug.DrawLine(transform.position, nextBlock.transform.position, Color.white, 2f);
        transform.position = nextBlock.transform.position;

    }

}
