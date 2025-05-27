import os

# Расширения файлов
wanted_exts = ['.json', '.cs']

# Папки, которые нужно пропустить
exclude_dirs = ['obj', '.git', 'Migrations', 'bin']

root = '.'

file_list = []
for dirpath, dirnames, filenames in os.walk(root):
    # Удаляем из dirnames исключённые папки (основной трюк)
    dirnames[:] = [d for d in dirnames if d not in exclude_dirs]
    for filename in filenames:
        if any(filename.endswith(ext) for ext in wanted_exts):
            full_path = os.path.join(dirpath, filename)
            file_list.append(full_path)

output = 'Отобранные файлы с нужными расширениями:\n'
for path in file_list:
    output += f'{path}\n'

output += '\n' + '-'*40 + '\n\n'

for path in file_list:
    output += f'// File {path}\n'
    try:
        with open(path, encoding='utf-8', errors='replace') as f:
            content = f.read()
    except Exception as e:
        content = f'[Ошибка чтения файла: {e}]'
    output += content + '\n'
    output += '\n' + '-'*40 + '\n\n'


print(output)