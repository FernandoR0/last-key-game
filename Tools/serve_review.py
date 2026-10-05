"""Local LAST KEY review/build server with byte ranges for real video seeking."""
import argparse, functools, os, re
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer

class ReviewHandler(SimpleHTTPRequestHandler):
    def end_headers(self):
        self.send_header('Accept-Ranges', 'bytes')
        super().end_headers()

    def send_head(self):
        self.byte_range = None
        path = self.translate_path(self.path)
        request = self.headers.get('Range')
        if not request or not os.path.isfile(path):
            return super().send_head()
        match = re.fullmatch(r'bytes=(\d*)-(\d*)', request.strip())
        file = open(path, 'rb')
        stat = os.fstat(file.fileno())
        size = stat.st_size
        if not match or not any(match.groups()):
            file.close()
            self.send_error(400, 'Single byte range required')
            return None
        first, last = match.groups()
        start = int(first) if first else max(0, size - int(last))
        end = min(size - 1, int(last)) if first and last else size - 1
        if start >= size or end < start:
            file.close()
            self.send_response(416)
            self.send_header('Content-Range', f'bytes */{size}')
            self.send_header('Content-Length', '0')
            self.end_headers()
            return None
        self.send_response(206)
        self.send_header('Content-Type', self.guess_type(path))
        self.send_header('Content-Range', f'bytes {start}-{end}/{size}')
        self.send_header('Content-Length', str(end - start + 1))
        self.send_header('Last-Modified', self.date_time_string(stat.st_mtime))
        self.end_headers()
        file.seek(start)
        self.byte_range = end - start + 1
        return file

    def copyfile(self, source, output):
        remaining = self.byte_range
        if remaining is None:
            return super().copyfile(source, output)
        while remaining:
            block = source.read(min(65536, remaining))
            if not block:
                break
            output.write(block)
            remaining -= len(block)

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--directory', required=True)
    parser.add_argument('--port', type=int, default=8765)
    args = parser.parse_args()
    handler = functools.partial(ReviewHandler, directory=args.directory)
    ThreadingHTTPServer(('127.0.0.1', args.port), handler).serve_forever()
