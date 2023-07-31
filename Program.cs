using System.ComponentModel;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
namespace BombsAway;

public class Program
{
    private bool started;
    private bool replace = false;
    private int timer = 0;
    private bool collided = false;
    private const int Scale = 4;
    private const int Columncount = 3;
    private float acceleration;
    private float y;
    private Random rnd;
    private readonly (float, bool)[] columns = new (float, bool)[4];
    private int wheight = 0;
    private int columnspan;
    private int gamestate = 0;
    private Image icon;
    private Color bgColor;

    private Texture2D bomb;
    private Texture2D tilescreen;
    private Texture2D characterselect;

    private Texture2D ame;
    private Texture2D gameOverAme;
    private Texture2D stageAme;


    private Texture2D gura;
    private Texture2D gameOverGura;
    private Texture2D stageGura;

    private Sound guraSound;
    private Sound ameSound;
    private Sound ameGameover;
    private Sound guraGameover;

    private Music ameStage;
    private Music guraStage;



    private Texture2D character;
    private Texture2D gameOverBackground;
    private Texture2D StageBackground;

    private Sound sound;
    private Sound gameOverSound;
    private Music stageMusic;



    private Music intro;
    private Music characterSelect;
    private Music logo;
    private float oldx = 0;
    private int score = 0;
    private int maxscore = 0;
    private const float Width = 800;
    private const float Height = 600;
    private RenderTexture2D rtex;
    private Font font;
    private Font fontlogo;
    private int cooldown;
    private bool isgura;

    public static void Main()
    {
        var porgram = new Program();
        porgram.Init();
        porgram.Run();
        porgram.Dispose();
    }

    private void Run()
    {
        while (!WindowShouldClose())
        {
            BeginTextureMode(rtex);
            ClearBackground(bgColor);
            switch (gamestate)
            {
                case 0:
                    Start();
                    break;
                case 1:
                    Stage();
                    break;
                case 2:
                    End();
                    break;
            }

            EndTextureMode();
            BeginDrawing();
            ClearBackground(bgColor);
            if (IsKeyPressed(KeyboardKey.KEY_ENTER))
            {
                if (IsWindowFullscreen())
                {
                    ToggleFullscreen();
                    SetWindowSize((int)Width, (int)Height);
                }
                else
                {
                    var monitor = GetCurrentMonitor();
                    var x = GetMonitorWidth(monitor);
                    var y = GetMonitorHeight(monitor);
                    SetWindowSize(x, y);
                    ToggleFullscreen();
                }
            }

            var aspect = (1.0f * GetScreenHeight() / GetScreenWidth()) / 0.75f;
            var srect = new Rectangle(0, (float)rtex.texture.height - Height, (float)rtex.texture.width, -Height);
            var drect = new Rectangle(GetScreenWidth() * (1 - aspect) * 0.5f, 0, GetScreenWidth() * aspect,
                GetScreenHeight());
            DrawTexturePro(rtex.texture, srect, drect, Vector2.Zero, 0, Color.WHITE);
            EndDrawing();
        }
    }

    private void Init()
    {
        rnd = new Random();
        InitWindow((int)Width, (int)Height, Resource1.BombsAway);
        icon = LoadImage(Path.Combine(Resource1.images, "bomb.png"));
        SetWindowIcon(icon);
        SetConfigFlags(ConfigFlags.FLAG_VSYNC_HINT);
        SetTargetFPS(60);
        InitAudioDevice();

        tilescreen = LoadTexture(Path.Combine(Resource1.images, "titlescreen.png"));
        characterselect = LoadTexture(Path.Combine(Resource1.images, "character_Select.png"));
        bomb = LoadTexture(Path.Combine(Resource1.images, "bomb.png"));


        ame = LoadTexture(Path.Combine(Resource1.images, "ame.png"));
        gameOverAme = LoadTexture(Path.Combine(Resource1.images, "game_over_ame.png"));
        stageAme = LoadTexture(Path.Combine(Resource1.images, "stage_ame.png"));

        gura = LoadTexture(Path.Combine(Resource1.images, "gura.png"));
        gameOverGura = LoadTexture(Path.Combine(Resource1.images, "game_over_gura.png"));
        stageGura = LoadTexture(Path.Combine(Resource1.images, "stage_gura.png"));


        ameSound = LoadSound(Path.Combine(Resource1.audio, "ame.mp3"));
        guraSound = LoadSound(Path.Combine(Resource1.audio, "gura.mp3"));
        ameGameover = LoadSound(Path.Combine(Resource1.audio, "game_over_ame.mp3"));
        guraGameover = LoadSound(Path.Combine(Resource1.audio, "game_over_gura.mp3"));
        ameStage = LoadMusicStream(Path.Combine(Resource1.audio, "stage_ame.mp3"));
        guraStage = LoadMusicStream(Path.Combine(Resource1.audio, "stage_gura.mp3"));
        intro = LoadMusicStream(Path.Combine(Resource1.audio, "intro.mp3"));
        characterSelect = LoadMusicStream(Path.Combine(Resource1.audio, "character select.mp3"));
        logo = LoadMusicStream(Path.Combine(Resource1.audio, "logo.mp3"));


        rtex = LoadRenderTexture((int)Width, (int)Width);
        font = LoadFontEx(Resource1.font, 115, null, 0);
        fontlogo = LoadFontEx(Resource1.fontlogo, 115, null, 0);
        GenTextureMipmaps(ref font.texture);
        SetTextureFilter(font.texture, TextureFilter.TEXTURE_FILTER_BILINEAR);
        gamestate = 0;

        bgColor = new Color(0, 0, 0, 0);
        Reset();

        PlayMusicStream(intro);
    }

    private void Dispose()
    {
        CloseAudioDevice();
        CloseWindow();
        UnloadImage(icon);
        UnloadTexture(ame);
        UnloadTexture(gura);


        UnloadTexture(bomb);
        UnloadTexture(tilescreen);
        UnloadTexture(characterselect);
        UnloadTexture(ame);
        UnloadTexture(gameOverAme);
        UnloadTexture(stageAme);
        UnloadTexture(gura);
        UnloadTexture(gameOverGura);
        UnloadTexture(stageGura);

        UnloadSound(guraSound);
        UnloadSound(ameSound);
        UnloadSound(ameGameover);
        UnloadSound(guraGameover);

        UnloadMusicStream(ameStage);
        UnloadMusicStream(guraStage);
        UnloadMusicStream(intro);
        UnloadMusicStream(characterSelect);
        UnloadMusicStream(logo);

        UnloadRenderTexture(rtex);
        UnloadFont(font);
        UnloadFont(fontlogo);
    }


    private void Start()
    {

        if (IsKeyPressed(KeyboardKey.KEY_SPACE))
        {
            gamestate = 1;
            Reset();
            CharSelect();
        }
        else
        {
            UpdateMusicStream(intro);

            DrawTexture(tilescreen, 0, 0, Color.WHITE);

            if ((timer / 20) % 2 == 0)
                DrawTextEx(font, Resource1.PressSpaceToStart,
                    new Vector2((int)(Width / 2 - MeasureTextEx(font, Resource1.PressSpaceToStart, 35, 0).X / 2),
                        (int)(Height * 0.8f)), 35, 0, Color.YELLOW);

            timer++;
        }

    }

    private void End()
    {
        if (IsKeyPressed(KeyboardKey.KEY_R))
        {
            StopMusicStream(stageMusic);
            PlayMusicStream(stageMusic);
            gamestate = 1;
            Reset();
        }
        else if (IsKeyPressed(KeyboardKey.KEY_B))
        {
            StopMusicStream(characterSelect);
            PlayMusicStream(characterSelect);
            StopSound(gameOverSound);
            gamestate = 0;
        }
        else
        {
            DrawTextureEx(gameOverBackground, Vector2.Zero, 0, Width / gameOverBackground.width, Color.WHITE);
            DrawTextEx(font, Resource1.PressRToRestart, new Vector2((Width * 0.53f), (Height - 165)), 35, 0,
                ((timer / 30) % 2 == 0) ? Color.YELLOW : Color.ORANGE);
            DrawTextEx(font, Resource1.PressBToReturnToMenu, new Vector2((Width * 0.53f), (Height - 130)), 35, 0,
                ((timer / 30) % 2 == 1) ? Color.YELLOW : Color.ORANGE);
            DrawTextEx(font, string.Format(Resource1.Score, score), new Vector2((Width * 0.53f), (Height - 95)), 35, 0,
                Color.YELLOW);
            DrawTextEx(font, string.Format(Resource1.MaxScore, maxscore), new Vector2((Width * 0.53f), (Height - 60)),
                35, 0, Color.YELLOW);

            timer++;
        }
    }

    private void Stage()
    {
        gamestate = 2;
        PlaySound(gameOverSound);
        //var character = smol ? beesmol : bee;
        //var framewith = (int)(character.width / 9f);

        //UpdateMusicStream(stagemusic);
        //if (IsKeyDown(KeyboardKey.KEY_SPACE))
        //{
        //    started = true;
        //    acceleration = -Scale;
        //    PlaySound(jump);
        //}
        //if (collided)
        //{
        //    collided = false;
        //    if (!smol && cooldown == 0)
        //    {
        //        cooldown = timer;
        //        smol = true;
        //        PlaySound(hurt);
        //    }
        //    if (smol && cooldown == 0)
        //    {
        //        if (score > maxscore)
        //        {
        //            maxscore = score;
        //        }

        //        PlaySound(ending);
        //        gamestate = 2;
        //    }
        //}
        //const float sch = Height / 2.0f;
        //var groundposition = new Vector2(0, Height - ground.height);
        //var playerposition = new Vector2(Width / 2.0f - 27, sch - 47 + y);
        //var playerbounds = new Rectangle(playerposition.X + 2, playerposition.Y + 2, framewith - 4f, character.height - 4f);
        //if (playerposition.Y + playerbounds.height > groundposition.Y)
        //    collided = true;
        //y = Math.Min(y, groundposition.Y - playerbounds.height + 47 - sch);
        //y = Math.Max(-250, y);
        //var cloudposition = new Vector2(0, Height * 0.35f);
        //var cloudsource = new Rectangle((int)((timer / 8f) % clouds1.width), 0, Width, clouds1.height);
        //DrawTextureRec(clouds1, cloudsource, cloudposition, Color.WHITE);
        //cloudsource = new Rectangle((int)((timer / 4f) % clouds2.width), 0, Width, clouds2.height);
        //DrawTextureRec(clouds2, cloudsource, cloudposition, Color.WHITE);
        //cloudsource = new Rectangle((int)((timer / 2f) % clouds3.width), 0, Width, clouds3.height);
        //DrawTextureRec(clouds3, cloudsource, cloudposition, Color.WHITE);
        //DrawRectangle(0, (int)(Height * 0.65), (int)Width, (int)(Height * 0.35), Color.WHITE);
        //for (var i = 0; i < Columncount; i++)
        //{
        //    var (y1, ishoney) = columns[i];
        //    y1 = -y1;
        //    var y2 = Height * 0.8f - columns[i].Item1;
        //    var x = columnspan * (i + 1) - timer % (columnspan) - wall.width;
        //    if (i == 0 && x == -wall.width + 1)
        //        replace = true;
        //    DrawTexture(wall, x, (int)y1, Color.WHITE);

        //    if (ishoney)
        //    {
        //        var honeyx = x - 10;
        //        var honeyy = (int)y1 + 350;
        //        DrawTexture(honey, honeyx, honeyy, Color.WHITE);
        //        if (CheckCollisionRecs(new Rectangle(honeyx, honeyy, honey.width, honey.height), playerbounds))
        //        {
        //            if (smol)
        //            {
        //                smol = false;
        //                PlaySound(grow);
        //            }
        //            else
        //            {
        //                score += 10;
        //                PlaySound(kaching);
        //            }

        //            columns[i] = (columns[i].Item1, false);
        //        }
        //    }

        //    DrawTexture(wall, x, (int)y2, Color.WHITE);

        //    if (CheckCollisionRecs(new Rectangle(x, y1, wall.width, wall.height), playerbounds) ||
        //        CheckCollisionRecs(new Rectangle(x, y2, wall.width, wall.height), playerbounds))
        //        collided = true;
        //}
        //if (replace)
        //{
        //    ReplaceColumn();
        //    replace = false;
        //    score++;
        //}
        //var source = new Rectangle(timer % ground.width, 0, Width, ground.height);
        //DrawTextureRec(ground, source, groundposition, Color.WHITE);
        //source = new Rectangle((timer % 9) * framewith, 0, framewith, character.height);
        //if (cooldown == 0 || (timer / 2) % 2 == 0)
        //    DrawTextureRec(character, source, playerposition, Color.WHITE);
        //var text = String.Format(Resource1.Score, score);
        //DrawTextEx(font, text, new Vector2(((Width - MeasureText(text, 35)) / 2), (Height - 50)), 35, 0, Color.YELLOW);

        //if (!started) return;
        //timer++;
        //acceleration += 0.1f * Scale;
        //y += acceleration;
        //if (cooldown != 0 && cooldown + 60 < timer)
        //    cooldown = 0;
    }

    private void Reset()
    {
        rnd = new Random();
        score = 0;
        collided = false;
        started = false;
        timer = 0;
        acceleration = 0;
        y = 0;
        for (var i = 0; i <= Columncount; i++)
            columns[i] = (rnd.Next(0, (int)(wheight * 0.9)), rnd.Next(0, 20) < 3);
    }

    private void CharSelect()
    {
        if (isgura)
        {
            character = gura;
            gameOverBackground = gameOverGura;
            StageBackground = stageGura;

            sound = guraSound;
            gameOverSound = guraGameover;
            stageMusic = guraStage;
        }
        else
        {

            character = ame;
            gameOverBackground = gameOverAme;
            StageBackground = stageAme;

            sound = ameSound;
            gameOverSound = ameGameover;
            stageMusic = ameStage;

        }
    }


    private void ReplaceColumn()
    {
        for (var i = 0; i <= Columncount - 1; i++)
            columns[i] = columns[i + 1];
        columns[Columncount - 1] = (rnd.Next(0, (int)(wheight * 0.9)), rnd.Next(0, 20) < 3);
    }
}