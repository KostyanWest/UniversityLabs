CC=gcc
CFLAGS=-c -Wall
LDFLAGS=
SOURCES=Sem2Lab7.c Sem2Lab7_list.c
LANGPOSTFIX=c
OBJECTS=$(SOURCES:.$(LANGPOSTFIX)=.o)
PROJECTNAME=Sem2Lab7

ifeq ($(OS),Windows_NT)
EXECUTABLE=$(PROJECTNAME).exe
RUN_ACTION=.\$(EXECUTABLE)
CLEAN_ACTION=del /q $(OBJECTS) $(EXECUTABLE)
else
EXECUTABLE=$(PROJECTNAME)
RUN_ACTION=./$(EXECUTABLE)
CLEAN_ACTION=rm -rf $(OBJECTS) $(EXECUTABLE)
endif


.PHONY: all run build clean

all: build

run: build
	$(RUN_ACTION)

build: $(SOURCES) $(EXECUTABLE)
	
$(EXECUTABLE): $(OBJECTS) 
	$(CC) $(LDFLAGS) $(OBJECTS) -o $(EXECUTABLE)

%.o: %.$(LANGPOSTFIX)
	$(CC) $(CFLAGS) $< -o $@

clean:
	$(CLEAN_ACTION)