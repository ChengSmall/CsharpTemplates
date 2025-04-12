using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cheng.GameTemplates.PushingBoxes;

namespace Cheng.GameTemplates.PushingBoxes
{

    /// <summary>
    /// 推箱子场景绘制器
    /// </summary>
#if UNITY_EDITOR
    [AddComponentMenu("Cheng/game/推箱子场景绘制器")]
#endif
    [DisallowMultipleComponent]
    public sealed class CameraDrawPushBoxScene : MonoBehaviour
    {

        #region

        public CameraDrawPushBoxScene()
        {
            drawRect = new Vector2(1, 1);
            Color initColor = Color.black;
            groundSprite.color = initColor;
            playerSprite.color = initColor;
            boxSprite.color = initColor;
            wallSprite.color = initColor;
            targetSprite.color = initColor;
            playerOnTargetSprite.color = initColor;
            boxOnTargetSprite.color = initColor;
            backgroundSprite.color = initColor;

            isTargetCover = false;
            drawing = true;
        }

        #endregion

        #region 参数

        #region 脚本参数

#if UNITY_EDITOR
        [Tooltip("绘制区域\nx参数表示左右两侧的绘制留白，1表示填满；y参数表示上下两侧绘制留白，1表示填满")]
#endif
        [SerializeField] private Vector2 drawRect;

#if UNITY_EDITOR
        [Tooltip("地面格子图像")]
#endif
        [SerializeField] private SpriteDraw groundSprite;


#if UNITY_EDITOR
        [Tooltip("玩家图像")]
#endif
        [SerializeField] private SpriteDraw playerSprite;

#if UNITY_EDITOR
        [Tooltip("箱子图像")]
#endif
        [SerializeField] private SpriteDraw boxSprite;

#if UNITY_EDITOR
        [Tooltip("墙壁图像")]
#endif
        [SerializeField] private SpriteDraw wallSprite;

#if UNITY_EDITOR
        [Tooltip("目标点图像")]
#endif
        [SerializeField] private SpriteDraw targetSprite;

#if UNITY_EDITOR
        [Tooltip("玩家处于目标点图像")]
#endif
        [SerializeField] private SpriteDraw playerOnTargetSprite;

#if UNITY_EDITOR
        [Tooltip("箱子处于目标点图像")]
#endif
        [SerializeField] private SpriteDraw boxOnTargetSprite;


#if UNITY_EDITOR
        [Tooltip("背景板图像")]
#endif
        [SerializeField] private SpriteDraw backgroundSprite;

#if UNITY_EDITOR
        [Tooltip("当有物品处于目标点上时，是否先绘制目标点再绘制物品")]
#endif
        [SerializeField] private bool isTargetCover;

#if UNITY_EDITOR
        [Tooltip("是否开启图像绘制")]
#endif
        [SerializeField] private bool drawing;

        #endregion

        #region 内部参数

        private PushBoxGame p_pushBoxGame;

        #endregion

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 地面格子图像
        /// </summary>
        public SpriteDraw GroundSprite
        {
            get => groundSprite;
            set => groundSprite = value;
        }

        /// <summary>
        /// 玩家绘制图像
        /// </summary>
        public SpriteDraw PlayerSprite
        {
            get => playerSprite;
            set
            {
                playerSprite = value;
            }
        }

        /// <summary>
        /// 箱子绘制图像
        /// </summary>
        public SpriteDraw BoxSprite
        {
            get => boxSprite;
            set => boxSprite = value;
        }

        /// <summary>
        /// 墙体图像
        /// </summary>
        public SpriteDraw WallSprite
        {
            get => wallSprite;
            set => wallSprite = value;
        }

        /// <summary>
        /// 目标点图像
        /// </summary>
        public SpriteDraw TargetSprite
        {
            get => targetSprite;
            set => targetSprite = value;
        }

        /// <summary>
        /// 玩家处于目标点图像
        /// </summary>
        public SpriteDraw PlayerOnTargetSprite
        {
            get => playerOnTargetSprite;
            set => playerOnTargetSprite = value;
        }

        /// <summary>
        /// 箱子处于目标点图像
        /// </summary>
        public SpriteDraw BoxOnTargetSprite
        {
            get => boxOnTargetSprite;
            set => boxOnTargetSprite = value;
        }

        /// <summary>
        /// 背景板绘制图像
        /// </summary>
        public SpriteDraw BackgroundSprite
        {
            get => backgroundSprite;
            set => backgroundSprite = value;
        }

        /// <summary>
        /// 绘制区域
        /// </summary>
        /// <remarks>
        /// <para>x参数表示左右两侧的绘制留白，1表示填满<br/>y参数表示上下两侧绘制留白，1表示填满</para>
        /// </remarks>
        public Vector2 DrawRect
        {
            get => drawRect;
            set
            {
                value.x = Mathf.Clamp01(value.x);
                value.y = Mathf.Clamp01(value.y);
                drawRect = value;
            }
        }

        /// <summary>
        /// 推箱子游戏操作台
        /// </summary>
        public PushBoxGame PushBoxGameCtrl
        {
            get => this.p_pushBoxGame;
            set
            {
                this.p_pushBoxGame = value;
            }
        }

        /// <summary>
        /// 当有物品处于目标点上时，是否先绘制目标点再绘制物品
        /// </summary>
        /// <value>参数为true时，先绘制目标点图像再绘制目标点上物体所表示的图像；参数为false时，直接绘制目标点上物体图像</value>
        public bool IsTargetDrawCover
        {
            get => isTargetCover;
            set => isTargetCover = value;
        }

        /// <summary>
        /// 是否开启图像绘制
        /// </summary>
        public bool Drawing
        {
            get => drawing;
            set
            {
                drawing = value;
            }
        }

        #endregion

        #endregion

        #region 运行

        #region editor

        private void OnValidate()
        {
            drawRect.x = Mathf.Clamp01(drawRect.x);
            drawRect.y = Mathf.Clamp01(drawRect.y);
        }

        #endregion

        #region 初始化和回收

        private void Awake()
        {
            p_pushBoxGame = new PushBoxGame();
        }

        private void Start()
        {
        }


        private void OnDestroy()
        {
            p_pushBoxGame = null;
        }

        #endregion

        #region 绘制

        static Rect verToRect(Vector2 vector, int width, int height)
        {
            //缩放参数
            var a = ((1 - vector.x) * 0.5f) * width;
            var b = ((1 - vector.y) * 0.5f) * height;
            return new Rect(a, b, width - (a * 2), height - (b * 2));
        }

        /// <summary>
        /// 将像素矩形根据像素长宽归一化标准矩形
        /// </summary>
        /// <param name="pixelRect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        static Rect pixelToRect(Rect pixelRect, int width, int height)
        {
            float x, y;
            if (pixelRect.x == 0f)
            {
                x = 0f;
            }
            else
            {
                x = pixelRect.x / width;
            }

            if (pixelRect.y == 0f)
            {
                y = 0f;
            }
            else
            {
                y = pixelRect.y / height;
            }

            float w, h;
            if (pixelRect.width == width)
            {
                w = 1f;
            }
            else
            {
                w = pixelRect.width / width;
            }
            if (pixelRect.height == height)
            {
                h = 1f;
            }
            else
            {
                h = pixelRect.height / height;
            }

            return new Rect(x, y, w, h);
        }

        static void drawGridSprite(SpriteDraw sprite, Rect rect, int width, int height)
        {
            Graphics.DrawTexture(rect, sprite.sprite.texture, pixelToRect(sprite.sprite.rect, width, height), 0, 0, 0, 0, sprite.color, null, -1);
        }

        /// <summary>
        /// 调用该函数绘制一次场景到屏幕
        /// </summary>
        public void DrawingScene()
        {
            var scene = p_pushBoxGame?.scene;
            if (scene is null) return;

            //this.backgroundTexture;
            var screen_width = Screen.width;
            var screen_height = Screen.height;

            var pixelRect = verToRect(this.drawRect, screen_width, screen_height);

            Graphics.DrawTexture(pixelRect, this.backgroundSprite.sprite.texture,
                pixelToRect(this.backgroundSprite.sprite.rect, screen_width, screen_height),
                0, 0, 0, 0,
                this.backgroundSprite.color, null, -1);

            if(scene.width == 0 || scene.height == 0)
            {
                return; //场景无体积，结束绘制
            }

            //1个格子所占像素长宽
            float widthOncePixel = pixelRect.width / (float)scene.width;

            float heightOncePixel = pixelRect.height / (float)scene.height;

            Rect drawGridRect;
            int x, y;

            for (y = 0; y < scene.height; y++)
            {
                for (x = 0; x < scene.width; x++)
                {
                    var grid = scene[x, y];

                    grid.GetValue(out SceneObject gridObj, out SceneTarget target, out SceneGround ground);

                    if (ground == SceneGround.Exist)
                    {
                        //存在地面
                        drawGridRect = new Rect(pixelRect.x + (x * widthOncePixel), (pixelRect.y + (((scene.height - 1) - y) * heightOncePixel)), widthOncePixel, heightOncePixel);


                        drawGridSprite(groundSprite, drawGridRect,
                                    screen_width, screen_height);

                        if (gridObj == SceneObject.None)
                        {
                            if (target == SceneTarget.Exist)
                            {
                                //终点格子
                                drawGridSprite(targetSprite, drawGridRect,
                                    screen_width, screen_height);
                            }
                        }
                        else if (gridObj == SceneObject.Wall)
                        {
                            //墙壁格子
                            drawGridSprite(wallSprite, drawGridRect,
                                screen_width, screen_height);

                        }
                        else if (gridObj == SceneObject.Player)
                        {
                            if (target == SceneTarget.Exist)
                            {
                                //存在目标点
                                if(isTargetCover) drawGridSprite(targetSprite, drawGridRect,
                                     screen_width, screen_height);
                            }
                            //玩家
                            drawGridSprite(playerSprite, drawGridRect,
                                screen_width, screen_height);
                        }
                        else if (gridObj == SceneObject.Box)
                        {
                            if (target == SceneTarget.Exist)
                            {
                                if (isTargetCover) drawGridSprite(targetSprite,
                                    drawGridRect,screen_width, screen_height);

                            }
                            
                            drawGridSprite(boxSprite, drawGridRect,
                                    screen_width, screen_height);
                        }
                        else
                        {
                            if (target == SceneTarget.Exist)
                            {
                                if (isTargetCover) drawGridSprite(targetSprite,
                                    drawGridRect, screen_width, screen_height);
                            }

                            OtherDrawGridTexture(grid, drawGridRect, 
                                screen_width, screen_height);
                        }
                        
                    }

                }

            }

        }


        private void OtherDrawGridTexture(SceneGrid grid, Rect rect, int width, int height)
        {
        }


        private void OnGUI()
        {
            var nowEvent = Event.current;

            var eventType = nowEvent.type;

            if (eventType == EventType.Repaint)
            {
                if(drawing) DrawingScene();

            }

        }

        #endregion


        #endregion

    }
}
#if UNITY_EDITOR

#endif