CC=gcc
CFLAGS=-c -Wall
LDFLAGS=
SOURCES=Sem2Lab6.c Sem2Lab6_EPN.c Sem2Lab6_EMNIA.c Sem2Lab6_BinTree.c
LANGPOSTFIX=c
OBJECTS=$(SOURCES:.$(LANGPOSTFIX)=.o)
EXECUTABLE=Sem2Lab6.exe

.PHONY: all run build clean

all: build

run: build
ifeq ($(OS),Windows_NT)
	.\$(EXECUTABLE)
else
	./$(EXECUTABLE)
endif

build: $(SOURCES) $(EXECUTABLE)
	
$(EXECUTABLE): $(OBJECTS) 
	$(CC) $(LDFLAGS) $(OBJECTS) -o $(EXECUTABLE)

%.o: %.$(LANGPOSTFIX)
	$(CC) $(CFLAGS) $< -o $@

clean:
ifeq ($(OS),Windows_NT)
	del /q $(OBJECTS) $(EXECUTABLE)
else
	rm -rf $(OBJECTS) $(EXECUTABLE)
endif