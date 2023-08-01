using System.ComponentModel;
using System.IO;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
namespace BombsAway;


internal enum GameState
{
    Logo,
    Start,
    CharSel,
    Stage,
    End,
    Credits
}
public class Program
{
    private int timer = 0;
    private int newbombcounter = 0;
    private int padding = 0;
    private bool collided = false;
    private Random rnd;
    private int bombcount = 0;
    private (Vector2, float)[] bombs;
    private Vector2 playerposition;
    private GameState gamestate = GameState.Logo;
    private Image icon;
    private Color bgColor;

    private Texture2D bomb;
    private Texture2D splashscreen;
    private Texture2D tilescreen;
    private Texture2D characterselect;
    private Texture2D creditscreen;
    private Texture2D credits;

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
    private Music creditmusic;



    private Texture2D character;
    private Texture2D gameOverBackground;
    private Texture2D stageBackground;

    private Sound sound;
    private Sound gameOverSound;
    private Music stageMusic;



    private Music intro;
    private Music characterSelect;
    private Sound logo;
    private int score = 0;
    private int maxscore = 0;
    private const float Width = 800;
    private const float Height = 600;
    private RenderTexture2D rtex;
    private Font font;
    private Font fontlogo;
    private bool isgura = true;
    private float mastervolume = 1;

    public static float Smoothstep(float x)
    {
        return x * x * (3 - 2 * x);
    }

    public static void Main()
    {
        Directory.SetCurrentDirectory(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]));
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
                case GameState.Logo:
                    Logo();
                    break;
                case GameState.Start:
                    Start();
                    break;
                case GameState.CharSel:
                    CharSel();
                    break;
                case GameState.Stage:
                    Stage();
                    break;
                case GameState.End:
                    End();
                    break;
                case GameState.Credits:
                    Credits();
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
            if (IsKeyDown(KeyboardKey.KEY_UP))
            {
                mastervolume = MathF.Min(1.0f, mastervolume + 0.005f);
                SetMasterVolume(mastervolume);
            }
            if (IsKeyDown(KeyboardKey.KEY_DOWN))
            {
                mastervolume = MathF.Max(0.0f, mastervolume - 0.005f);
                SetMasterVolume(mastervolume);
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

        splashscreen = LoadTexture(Path.Combine(Resource1.images, "logo.png"));
        tilescreen = LoadTexture(Path.Combine(Resource1.images, "titlescreen.png"));
        characterselect = LoadTexture(Path.Combine(Resource1.images, "character_Select.png"));
        bomb = LoadTexture(Path.Combine(Resource1.images, "bomb.png"));
        creditscreen = LoadTexture(Path.Combine(Resource1.images, "hospital_inside.png"));
        credits = LoadTexture(Path.Combine(Resource1.images, "credits.png"));


        ame = LoadTexture(Path.Combine(Resource1.images, "ame.png"));
        SetTextureFilter(ame, TextureFilter.TEXTURE_FILTER_BILINEAR);
        GenTextureMipmaps(ref ame);
        gameOverAme = LoadTexture(Path.Combine(Resource1.images, "game_over_ame.png"));
        stageAme = LoadTexture(Path.Combine(Resource1.images, "stage_ame.png"));

        gura = LoadTexture(Path.Combine(Resource1.images, "gura.png"));
        SetTextureFilter(gura, TextureFilter.TEXTURE_FILTER_BILINEAR);
        GenTextureMipmaps(ref gura);
        gameOverGura = LoadTexture(Path.Combine(Resource1.images, "game_over_gura.png"));
        stageGura = LoadTexture(Path.Combine(Resource1.images, "stage_gura.png"));


        ameSound = LoadSound(Path.Combine(Resource1.audio, "ame.ogg"));
        guraSound = LoadSound(Path.Combine(Resource1.audio, "gura.ogg"));
        ameGameover = LoadSound(Path.Combine(Resource1.audio, "game_over_ame.ogg"));
        guraGameover = LoadSound(Path.Combine(Resource1.audio, "game_over_gura.ogg"));
        ameStage = LoadMusicStream(Path.Combine(Resource1.audio, "stage_ame.ogg"));
        guraStage = LoadMusicStream(Path.Combine(Resource1.audio, "stage_gura.ogg"));
        intro = LoadMusicStream(Path.Combine(Resource1.audio, "intro.ogg"));
        characterSelect = LoadMusicStream(Path.Combine(Resource1.audio, "character select.ogg"));
        creditmusic = LoadMusicStream(Path.Combine(Resource1.audio, "credits.ogg"));
        logo = LoadSound(Path.Combine(Resource1.audio, "logo.ogg"));


        rtex = LoadRenderTexture((int)Width, (int)Width);
        font = LoadFontEx(Resource1.font, 115, null, 0);
        fontlogo = LoadFontEx(Resource1.fontlogo, 115, null, 0);
        GenTextureMipmaps(ref font.texture);
        SetTextureFilter(font.texture, TextureFilter.TEXTURE_FILTER_BILINEAR);
        gamestate = 0;

        bgColor = new Color(0, 0, 0, 0);
        Reset();
        if (!File.Exists(Resource1.Scorefile))
            return;
        var scorefile = File.ReadAllText(Resource1.Scorefile);
        if (string.IsNullOrEmpty(scorefile))
            return;
        score = int.Parse(scorefile);
    }

    private void Dispose()
    {
        CloseAudioDevice();
        CloseWindow();
        UnloadImage(icon);
        UnloadTexture(ame);
        UnloadTexture(gura);
        UnloadTexture(creditscreen);
        UnloadTexture(credits);

        UnloadTexture(bomb);
        UnloadTexture(splashscreen);
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
        UnloadMusicStream(creditmusic);
        UnloadSound(logo);

        UnloadRenderTexture(rtex);
        UnloadFont(font);
        UnloadFont(fontlogo);
    }


    private void Logo()
    {
        if (360 < timer)
        {
            gamestate = GameState.Start;
            timer = 0;
            return;
        }

        if (timer == 0)
            PlaySound(logo);

        ClearBackground(timer < 120 || 240 < timer ? Color.BLACK : Color.WHITE);

        timer++;
        var fract = Smoothstep(MathF.Min(1f, MathF.Max(0f, timer / 120f)));
        var fract2 = Smoothstep(MathF.Min(1f, MathF.Max(0f, (timer - 120) / 120f)));
        var fade = Smoothstep(MathF.Min(1f, MathF.Max(0f, (360f - timer) / 120)));

        var startposx = Width / 2 - splashscreen.width / 2;
        var startposy = Height / 2 - splashscreen.height / 2;
        var color = new Color(255, 255, 255, (int)(255 * fract * fade));
        var color2 = new Color(203, 206, 249, (int)(255 * fract2 * fade));

        Raylib.DrawTexture(splashscreen, (int)startposx, (int)startposy, color);
        DrawTextEx(fontlogo, Resource1.Presents, new Vector2((int)(Width / 2 - MeasureTextEx(font, Resource1.Presents, 55, 0).X / 2), (int)(Height * 0.70f)), 55, 0, color2);

    }

    private void Start()
    {
        if (timer == 0)
        {
            StopMusicStream(intro);
            PlayMusicStream(intro);
        }

        if (IsKeyPressed(KeyboardKey.KEY_SPACE))
        {
            gamestate = GameState.CharSel;
            timer = 0;
            return;
        }
        if (IsKeyPressed(KeyboardKey.KEY_C))
        {
            gamestate = GameState.Credits;
            timer = 0;
            return;
        }
        else
        {
            UpdateMusicStream(intro);

            DrawTexture(tilescreen, 0, 0, Color.WHITE);

            if ((timer / 20) % 4 == 0)
                DrawTextEx(font, Resource1.PressSpaceToStart,
                    new Vector2((int)(Width / 2 - MeasureTextEx(font, Resource1.PressSpaceToStart, 35, 0).X / 2),
                        (int)(Height * 0.85f)), 35, 0, Color.SKYBLUE);

            if ((timer / 20) % 4 == 2)
                DrawTextEx(font, Resource1.CForCredits,
                    new Vector2((int)(Width / 2 - MeasureTextEx(font, Resource1.CForCredits, 35, 0).X / 2),
                        (int)(Height * 0.85f)), 35, 0, Color.SKYBLUE);

            timer++;
        }

    }

    private void CharSel()
    {
        if (timer == 0)
        {
            StopMusicStream(characterSelect);
            PlayMusicStream(characterSelect);
            CharacterSelected();
            PlaySound(sound);
        }

        if (IsKeyPressed(KeyboardKey.KEY_LEFT) || IsKeyPressed(KeyboardKey.KEY_RIGHT)
            || IsKeyPressed(KeyboardKey.KEY_A) || IsKeyPressed(KeyboardKey.KEY_D))
        {
            isgura = !isgura;
            CharacterSelected();
            PlaySound(sound);
        }
        if (IsKeyPressed(KeyboardKey.KEY_SPACE))
        {
            gamestate = GameState.Stage;
            Reset();
            return;
        }

        DrawTexture(characterselect, 0, 0, Color.WHITE);
        UpdateMusicStream(characterSelect);
        if ((timer / 20) % 2 == 0 || !isgura)
        {
            DrawTextEx(font, Resource1.Gura, new Vector2(Width * .33f - MeasureTextEx(font, Resource1.Gura, 35, 0).X * 0.5f, 150)
                , 35, 0, Color.SKYBLUE);
        }
        if ((timer / 20) % 2 == 0 || isgura)
        {
            DrawTextEx(font, Resource1.Ame, new Vector2(Width * .66f - MeasureTextEx(font, Resource1.Ame, 35, 0).X * 0.5f, 150)
                , 35, 0, Color.SKYBLUE);
        }

        DrawTexture(gura, (int)(Width * .33f - gura.width * 0.5f), 250, Color.WHITE);
        DrawTexture(ame, (int)(Width * .66f - ame.width * 0.5f), 250, Color.WHITE);
        timer++;

    }

    private void End()
    {
        if (IsKeyPressed(KeyboardKey.KEY_R))
        {
            StopSound(gameOverSound);
            gamestate = GameState.Stage;
            Reset();
            return;
        }
        else if (IsKeyPressed(KeyboardKey.KEY_B))
        {
            StopSound(gameOverSound);
            gamestate = GameState.Start;
            timer = 0;
            isgura = true;
            CharacterSelected();
            return;
        }
        else
        {
            DrawTextureEx(gameOverBackground, Vector2.Zero, 0, Width / gameOverBackground.width, Color.WHITE);
            DrawTextEx(font, Resource1.PressRToRestart, new Vector2((Width * 0.53f), (Height - 165)), 35, 0,
                ((timer / 30) % 2 == 0) ? Color.SKYBLUE : Color.BLUE);
            DrawTextEx(font, Resource1.PressBToReturnToMenu, new Vector2((Width * 0.53f), (Height - 130)), 35, 0,
                ((timer / 30) % 2 == 1) ? Color.SKYBLUE : Color.BLUE);
            DrawTextEx(font, string.Format(Resource1.Score, score), new Vector2((Width * 0.53f), (Height - 95)), 35, 0,
                Color.SKYBLUE);
            DrawTextEx(font, string.Format(Resource1.MaxScore, maxscore), new Vector2((Width * 0.53f), (Height - 60)),
                35, 0, Color.SKYBLUE);

            timer++;
        }
    }

    private void Credits()
    {
        if (timer == 0)
        {
            StopMusicStream(creditmusic);
            PlayMusicStream(creditmusic);
        }
        if (IsKeyPressed(KeyboardKey.KEY_B))
        {
            StopSound(gameOverSound);
            gamestate = GameState.Start;
            timer = 0;
            isgura = true;
            CharacterSelected();
            return;
        }
        UpdateMusicStream(creditmusic);
        DrawTexture(creditscreen, 0, 0, Color.WHITE);

        DrawTexture(credits,(int)Width/2 - credits.width/ 2,400 -timer,Color.WHITE);

        DrawTextEx(font, Resource1.PressBToReturnToMenu, new Vector2(Width * 0.5f - MeasureTextEx(font, Resource1.PressBToReturnToMenu, 35, 0).X * 0.5f, (Height - 100)), 35, 0,
            ((timer / 30) % 2 == 1) ? Color.SKYBLUE : Color.BLUE);
        timer++;
    }
    private void Stage()
    {
        if (timer == 0)
        {
            StopMusicStream(stageMusic);
            PlayMusicStream(stageMusic);
            playerposition = new Vector2(MathF.Floor(Width * .5f - character.width * 0.5f), 500);
        }
        if (collided)
        {
            gamestate = GameState.End;
            PlaySound(gameOverSound);
            score = timer;
            if (score < 100)
                score = 69;//NICE
            maxscore = Math.Max(score, maxscore);
            File.WriteAllText(Resource1.Scorefile, maxscore.ToString());
        }
        var movement = 5f + bombcount * 0.2f;
        if (IsKeyDown(KeyboardKey.KEY_LEFT) || IsKeyDown(KeyboardKey.KEY_A))
        {
            playerposition = playerposition with { X = MathF.Floor(MathF.Max(0.0f, playerposition.X - movement)) };
        }
        if (IsKeyDown(KeyboardKey.KEY_RIGHT) || IsKeyDown(KeyboardKey.KEY_D))
        {
            playerposition = playerposition with { X = MathF.Floor(MathF.Min(Width - character.width * 0.5f, playerposition.X + movement)) };
        }
        UpdateMusicStream(stageMusic);
        DrawTexture(stageBackground, 0, 0, Color.WHITE);

        DrawTextureEx(character, playerposition, 0, .5f, Color.WHITE);
        DrawTextEx(font, string.Format(Resource1.Score, timer), new Vector2(10, 10), 35, 0, Color.SKYBLUE);

        UpdateBombs();

        timer++;

    }

    private void UpdateBombs()
    {
        if (timer % newbombcounter == (newbombcounter - 1))
            bombcount = Math.Min(bombs.Length, bombcount + 1);
        var prect = new Rectangle(playerposition.X + padding, playerposition.Y + padding, character.width * .5f - padding * 2, character.height * .5f - padding * 2);
        //DrawRectangleLines((int)playerposition.X + padding, (int)playerposition.Y + padding, character.width/2 -padding*2, character.height/2 -padding*2, Color.RED);
        for (int i = 0; i < bombcount; i++)
        {
            if (bombs[i].Item1.Y > 600 || bombs[i].Item1.X == 0)
            {
                bombs[i] = (new Vector2(rnd.NextSingle() * Width, -40f), 0f);
                continue;
            }
            else
            {
                var speed = bombs[i].Item2 += 0.216f;
                var pos = bombs[i].Item1 with { Y = bombs[i].Item1.Y + bombs[i].Item2 };
                bombs[i] = (pos, speed);

                var bombrect = new Rectangle(pos.X + 2, pos.Y + 2, bomb.width - 14, bomb.height - 2);
                //DrawRectangleLines((int)pos.X +2, (int)pos.Y +2, bomb.width -14, bomb.height -2,Color.RED);

                if (CheckCollisionRecs(bombrect, prect))
                    collided = true;
                DrawTextureEx(bomb, bombs[i].Item1, 0, 1, Color.WHITE);

            }
        }
    }


    private void Reset()
    {
        rnd = new Random();
        score = 0;
        collided = false;
        timer = 0;
        bombs = new (Vector2, float)[200];
        bombcount = 1;
    }

    private void CharacterSelected()
    {
        if (isgura)
        {
            character = gura;
            gameOverBackground = gameOverGura;
            stageBackground = stageGura;

            sound = guraSound;
            gameOverSound = guraGameover;
            stageMusic = guraStage;
            newbombcounter = 345;
            padding = 10;
        }
        else
        {

            character = ame;
            gameOverBackground = gameOverAme;
            stageBackground = stageAme;

            sound = ameSound;
            gameOverSound = ameGameover;
            stageMusic = ameStage;
            newbombcounter = 138;
            padding = 2;
        }
    }
}