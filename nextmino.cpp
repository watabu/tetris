#include "DxLib.h"
#define width 1920
#define height 1080

class Mino{
private:
	int mino[15] = {};// ƒ~ƒm‚Ìí—Ş‚Í‚O‚©‚ç‚U‚Ì‚Ví—ŞA‚P‚©‚ç‚P‚S‚ªŸ‚Ìƒ~ƒm
public:
	void shuffleMino();//Ÿ‚Ìƒ~ƒmƒŠƒXƒg‚ğƒVƒƒƒbƒtƒ‹‚·‚é

	int getNextMino(); //Ÿ‚Ìƒ~ƒm‚ğ•Ô‚·
	void DrawMino();//ƒeƒXƒg—p
};

int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int) {

	//ChangeWindowMode(TRUE);
	SetGraphMode(width, height, 16), SetMainWindowText("Tetorinu");
	DxLib_Init();
	SetDrawScreen(DX_SCREEN_BACK);


	Mino mino;
	mino.shuffleMino();
	int flag = 0;
	int mino2 = mino.getNextMino();
	while (ScreenFlip() == 0 && ProcessMessage() == 0 && ClearDrawScreen() == 0 && CheckHitKey(KEY_INPUT_ESCAPE) == 0) {
	
		if (CheckHitKey(KEY_INPUT_SPACE) != 0) {
			if (flag == 0) {
				mino2 = mino.getNextMino();
			}
			flag = 1;
		}
		else {
			flag = 0;
		}
	
		
		DrawFormatString(100, 100, GetColor(255, 255, 0), "mino is %d", mino2);
		mino.DrawMino();
	}
	DxLib_End();
	return 0;
}



void Mino::shuffleMino() {
	int flag[15] = {};//‘ã“ü‚³‚ê‚Ä‚È‚¢‚È‚ç‚O
	int num;
	for (int i = 1; i < 15;) {
		num = GetRand(13) + 1;//‚P`14‚Ü‚Å
		if (flag[num] == 0) {//‘ã“ü‚³‚ê‚Ä‚È‚¯‚ê‚Î
			mino[num] = i % 7;//í—Ş‚ğ‘ã“ü@‚Qí—Ş‚¸‚Â”z’u‚³‚ê‚é
			flag[num] = 1;
			i++;
		}
	}
}

int Mino::getNextMino() {
	static int num = 0;
	num++;
	if (num > 14) {
		num = 1;
		shuffleMino();

	}
	return mino[num];//ƒ~ƒm‚Ìí—Ş‚ğ•Ô‚·

}

void Mino::DrawMino() {
	for (int i = 1; i < 15; i++) {
		DrawFormatString(200, 100 + 30 * i, GetColor(255, 255, 0), "Mino %d is %d", i, mino[i]);
	}
}