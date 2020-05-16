/*
Lang	C
Sem		2
Lab		7
Task	6.2 (17)
*/

#pragma warning(disable : 4996)

#include <stdio.h>
#include <stdlib.h>
#include <conio.h>
#include <Windows.h>
#include <string.h>

#include "Sem2Lab7_list.h"

/* Показать результаты */
void ShowResults ()
{
	FILE* f;
	char str[256];
	str[255] = '\0';

	f = fopen("Results.txt", "rt");
	if (f == NULL) {
		printf("Ошибка: не удалось открыть файл \"Results.txt\"!\n");
		_getch();
		return;
	}
	system("cls");
	printf(" === Список участников ===\n");
	while (1) {
		/* Считываем строку */
		for (int i = 0; ; i++) {
			if (i >= 256 || fread(str + i, 1, 1, f) <= 0) {
				fclose(f);
				_getch();
				return;
			}
			if (str[i] == '\n') {
				str[i] = '\0';
				break;
			}
		}
		printf("%s\n", str);
	}
}

/* Сохранить результат */
void SaveResult (const char* const name, const int score)
{
	FILE* f;
	f = fopen("Results.txt", "a+t");
	fprintf(f, "%s\t%d\n", name, score);
	fclose(f);
	return;
}

/* Выбор строчки меню 
	return:
	min - max - выбрана строчка меню
	-1 - подтверждение выбора
	*/
int SelectMenu (int min, int max, int current)
{
	char key;

	while (1)
	{
		key = _getch();
		switch (key) {
		case 'w':
		case 'W':
		case 'ц':
		case 'Ц':
			return (current > min) ? current - 1 : max;
		case 's':
		case 'S':
		case 'ы':
		case 'Ы':
			return (current < max) ? current + 1 : min;
		case '\n':
		case 'e':
		case 'E':
		case 'у':
		case 'У':
			return -1;
		}
	}
}

/* Выбор случайных вопросов */
void ChooseRandomTasks (int* const pTasksNums, const int count, const int maxCount)
{
	for (int i = 0; i < count; i++) {
		/* Уникальные случайные числа */
		int num = rand() % (maxCount - i);
		for (int j = 0; j < i; j++) {
			for (int k = 0; k < i; k++) {
				if (pTasksNums[k] == num) {
					num++;
					break;
				}
			}
		}
		pTasksNums[i] = num;
	}
}

/* Выбор вопросов
	return:
	0 - всё хорошо
	-1 - слишком мало вопросов
	-3 - не достаточно памяти (malloc)
	*/
int ChooseTasks (Task** const ppFirstTask, int** const ppTasksNums, const int count)
{
	int maxCount = 0;
	Task* pTemp = *ppFirstTask;
	while (pTemp != NULL) {
		pTemp = pTemp->pNext;
		maxCount++;
	}
	if (maxCount >= count) {
		if ((*ppTasksNums = malloc(count * sizeof(**ppTasksNums))) == NULL) {
			return -3;
		}
		ChooseRandomTasks(*ppTasksNums, count, maxCount);
		return 0;
	} else {
		return -1;
	}
}

/* Загрузка вопроса
	return:
	0 - всё хорошо
	-1 - не удалось открыть файл
	-2 - файл повреждён
	-3 - не достаточно памяти (malloc)
	*/
int LoadTasks (Task** const ppFirstTask) {
	FILE* f;
	char* pTempAnswers[5];
	Task* pTempTask = NULL;
	int answersCount = 0;

	f = fopen("Tasks.txt", "rt");
	if (f == NULL) {
		return -1;
	}
	while (1) {
		char str[1024];
		str[1023] = '\0';
		/* Считываем строку */
		for (int i = 0; ; i++) {
			if (i >= 1024 || fread(str + i, 1, 1, f) <= 0) {
				fclose(f);
				if (pTempTask != NULL) {
					pTempTask->answersCount = answersCount;
					if ((pTempTask->answers = malloc (sizeof (char*) * answersCount)) == NULL) {
						return -3;
					}
					for (int i = 0; i < answersCount; i++) {
						pTempTask->answers[i] = pTempAnswers[i];
					}
					AddTask(ppFirstTask, pTempTask);
				}
				return 0;
			}
			if (str[i] == '\n') {
				str[i] = '\0';
				break;
			}
		}
		/* Нашли вопрос */
		if (strcmp(str, "Q") == 0) {
			if (pTempTask != NULL) {
				pTempTask->answersCount = answersCount;
				if ((pTempTask->answers = malloc (sizeof (char*) * answersCount)) == NULL) {
					fclose(f);
					return -3;
				}
				for (int i = 0; i < answersCount; i++) {
					pTempTask->answers[i] = pTempAnswers[i];
				}
				AddTask(ppFirstTask, pTempTask);
			}
			answersCount = 0;
			if ((pTempTask = malloc(sizeof(Task))) == NULL) {
				fclose(f);
				return -3;
			}
			pTempTask->question = NULL;
			pTempTask->answers = NULL;
			for (int i = 0; ; i++) {
				if (i >= 1024 || fread(str + i, 1, 1, f) <= 0) {
					fclose(f);
					return -2;
				}
				if (str[i] == '$') {
					str[i] = '\0';
					break;
				}
			}
			char* pTempQuestion;
			if ((pTempQuestion = malloc(sizeof(char) * (strlen(str) + 1))) == NULL) {
				fclose(f);
				return -3;
			}
			strcpy(pTempQuestion, str);
			pTempTask->question = pTempQuestion;
			continue;
		}
		/* Нашли правильный ответ */
		if (strcmp(str, "RA") == 0) {
			if (pTempTask == NULL) {
				fclose(f);
				return -2;
			}
			pTempTask->rightAnswer = answersCount;
			str[0] = 'A';
			str[1] = '\0';
		}
		/* Нашли ответ */
		if (strcmp(str, "A") == 0) {
			if (pTempTask == NULL || answersCount >= 5) {
				fclose(f);
				return -2;
			}
			/* Считываем строку */
			for (int i = 0; ; i++) {
				if (i >= 1024 || fread(str + i, 1, 1, f) <= 0) {
					fclose(f);
					return -2;
				}
				if (str[i] == '\n') {
					str[i] = '\0';
					break;
				}
			}
			char* pTempAnswer;
			if ((pTempAnswer = malloc(sizeof(char) * (strlen(str) + 1))) == NULL) {
				fclose(f);
				return -3;
			}
			strcpy(pTempAnswer, str);
			pTempAnswers[answersCount] = pTempAnswer;
			answersCount++;
			continue;
		}
	}
}

/* Тестирование */
void Testing (void)
{
	char name[64];
	Task* pFirstTask = NULL;
	int* pTasksNums;
	const int tasksCount = 10;
	int select;
	int nextSelect;
	int score;

	/* Ввод имени */
	system("cls");
	printf(" === Начало тестирования ===\n");
	printf("Введите ваше имя: (50 символов)\n");
	/* Считываем строку */
	for (int i = 0; ; i++) {
		if (i >= 64) {
			name[63] = '\0';
			break;
		}
		name[i] = getchar();
		if (name[i] == '\n') {
			name[i] = '\0';
			break;
		}
	}


	/* Выбор вопросов */
	select = LoadTasks(&pFirstTask);
	if (select < 0) {
		system("cls");
		printf("Ошибка: ");
		if (select == -1) {
			printf("не удалось открыть файл \"Tasks.txt\"!\n");
		} else if (select == -2) {
			printf("файл \"Tasks.txt\" повреждён!\n");
		} else if (select == -3) {
			printf("недостаточно памяти!\n");
		}
		FreeTasks(&pFirstTask);
		_getch();
		return;
	}

	select = ChooseTasks(&pFirstTask, &pTasksNums, tasksCount);
	if (select < 0) {
		system("cls");
		printf("Ошибка: ");
		if (select == -1) {
			printf("слишком мало вопросов!\n");
		} else if (select == -3) {
			printf("недостаточно памяти!\n");
		}
		FreeTasks(&pFirstTask);
		_getch();
		return;
	}

	/* Ответы на вопросы */
	score = 0;
	for (int i = 0; i < tasksCount; i++) {
		Task* pTemp = GotoTask(pFirstTask, pTasksNums[i]);
		select = 0;

		while (1) {
			system("cls");
			printf(" === Вопрос %d ===\n", i + 1);
			puts(pTemp->question);
			printf(" ------------------------------\n\n");
			for (int j = 0; j < pTemp->answersCount; j++) {
				if (select == j)
					printf(" >>");
				printf("\t%d. %s\n", j + 1, pTemp->answers[j]);
			}
			nextSelect = SelectMenu(0, pTemp->answersCount - 1, select);
			if (nextSelect == -1) {
				if (select == pTemp->rightAnswer) {
					score++;
				}
				break;
			} else {
				select = nextSelect;
			}
		}
	}

	FreeTasks(&pFirstTask);
	free(pTasksNums);

	/* Результаты */
	system("cls");
	printf(" === Результаты ===\n");
	printf("%s\n", name);
	printf("Правильных ответов %d из %d\n", score, tasksCount);
	SaveResult(name, score);
	_getch();
}

/* Главное меню */
void MainMenu (void)
{
	int select = 0;
	int nextSelect;

	while (1) {
		system("cls");
		printf(" === Система тестирования по языку Си ===\n");
		if (select == 0)
			printf(" >>");
		printf("\t1. Начать тест\n");
		if (select == 1)
			printf(" >>");
		printf("\t2. Список участников\n");
		if (select == 2)
			printf(" >>");
		printf("\t3. Выход\n");
		nextSelect = SelectMenu(0, 2, select);
		if (nextSelect == -1) {
			switch (select) {
			case 0:
				Testing();
				break;
			case 1:
				ShowResults();
				break;
			case 2:
				return;
			}
		} else {
			select = nextSelect;
		}
	}
}

/* Main */
int main (void)
{
	SetConsoleCP (1251);
	SetConsoleOutputCP (1251);

	MainMenu();

	return 0;
}